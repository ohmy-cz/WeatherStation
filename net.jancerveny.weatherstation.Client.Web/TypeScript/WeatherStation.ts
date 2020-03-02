/// <reference types="./chart.js" >

enum ChartTypeEnum {
	RealTime,
	Monthly,
	Weekly,
	Last7Days,
	Annual
}

interface IChartJsLabels {
	label: string,
	span: number,
	start: Date
}

interface IChartConfiguration {
	name: string,
	chartType: ChartTypeEnum,
	labels: IChartJsLabels[]
}

interface IDataSource {
	id: number,
	cds: Chart.ChartDataSets
}

interface IMeasurement {
	id: number,
	timestamp: Date,
	sourceId: number,
	temperature: number
}

interface IAggregatedMeasurement {
	id: number,
	day: Date,
	sourceId: number,
	temperature: number
}

interface IReadOut {
	sourceId: number,
	temperature: number
}

interface IReadingsResponse {
	timestamp: Date,
	readouts: IReadOut[]
}

jQuery(function ($) {
	var readoutsSince = null;
	var dataSources1:IDataSource[] = [];

	var getChartsConfig = function (cb) {
		$.get('/data/GetChartsConfig', function (r: IChartConfiguration[]) {
			if (cb) {
				cb(r);
			}
		});
	};

	var getDataSources = function (cb) {
		$.get('/data/GetDataSources', function (r) {
			if (cb) {
				cb(r);
			}
		});
	};

	var getDataReadouts = function (cb) {
		$.get('/data/GetReadouts', { since: readoutsSince }, function (r: IReadingsResponse) {
			readoutsSince = r.timestamp; //  Makes sure we only get the delta
			if (cb) {
				cb(r.readouts);
			}
		});
	};

	var getAggregationReadouts = function (cb) {
		$.get('/data/GetAggregations', function (r: IAggregatedMeasurement[]) {
			if (cb) {
				cb(r);
			}
		});
	};

	var getChartsDataSources = function (cb) {
		if (!cb) {
			return;
		}
		getDataSources((dataSources) => {
			for (var i in dataSources) {
				dataSources1.push({
					id: dataSources[i].id,
					cds: {
						label: dataSources[i].name,
						backgroundColor: `rgba(${dataSources[i].color.r}, ${dataSources[i].color.g}, ${dataSources[i].color.b}, ${dataSources[i].color.a})`,
						borderColor: `rgba(${dataSources[i].color.r}, ${dataSources[i].color.g}, ${dataSources[i].color.b}, 1)`,
						fill: false,
						borderWidth: 3,
						lineTension: 0.1
					}
				});
			}
			cb(dataSources1);
		});
	};

	const createChart = (chartConfig: IChartConfiguration, datasets: Chart.ChartDataSets[]): Chart => {
		let labels: string[] = chartConfig.labels.map(l => l.label);
		if (labels.length === 0) {
			return;
		}

		var canvas: HTMLCanvasElement = document.getElementById(ChartTypeEnum[chartConfig.chartType]) as HTMLCanvasElement;
		var ctx = canvas.getContext('2d');
		return new Chart(ctx, {
			type: 'line',
			data: {
				labels,
				datasets
			},
			options: {
				title: {
					display: true,
					text: chartConfig.name
				}
			}
		});
	}

	// Initial render
	getChartsConfig((chartConfig: IChartConfiguration[]) => getChartsDataSources((ds: IDataSource[]) => {
		chartConfig.forEach(c => {
			if (c.chartType === ChartTypeEnum.RealTime) {
				getDataReadouts((dr: IReadOut[]) => {
					let concreteDataSet = ds.map(d => {
						let temperatures = dr.filter(dr1 => dr1.sourceId == d.id).map(dr1 => dr1.temperature / 100);

						// align values to right
						while (temperatures.length < c.labels.length) {
							temperatures.unshift(null);
						}

						var ncds: Chart.ChartDataSets = {
							label: d.cds.label,
							backgroundColor: d.cds.backgroundColor,
							borderColor: d.cds.borderColor,
							fill: d.cds.fill,
							borderWidth: d.cds.borderWidth,
							lineTension: d.cds.lineTension,
							data: temperatures
						}
						return ncds;
					});

					let realtimeChart = createChart(c, concreteDataSet);



					setInterval(function () {
						getDataReadouts((delta: IReadOut[]) => {
							if (!delta || delta.length === 0) {
								return;
							}

							// make space for the new column
							realtimeChart.data.datasets.forEach((dataset) => {
								dataset.data.splice(0, 1);
							});

							//  Add a new column
							realtimeChart.data.datasets.forEach((dataset) => {
								let dsf = dataSources1.filter(x => x.cds.label == dataset.label);
								if (dsf.length == 0) {
									return;
								}
								let datasetId = dsf[0].id;
								let df = delta.filter(x => x.sourceId == datasetId);
								//let lastKnownValue: number | null = typeof dataset.data[dataset.data.length - 1] === "number" ? dataset.data[dataset.data.length - 1] as number : null; // Handled on the server.
								let lastKnownValue = null;
								let temp: number | null = df.length > 0 ? (df[0].temperature / 100) : lastKnownValue;

								console.log(`Adding column to: ${dataset.label} temp: ${temp}`);
								dataset.data.push(temp);
							});
							realtimeChart.update();
						});
					}, 10 * 1000);
				});
			}

			if (c.chartType === ChartTypeEnum.Last7Days) {
				getAggregationReadouts((dr: IAggregatedMeasurement[]) => {
					let concreteDataSet = ds.map(d => {
						let temperatures = dr.filter(dr1 => dr1.sourceId == d.id).map(dr1 => dr1.temperature / 100);

						// align values to right
						while (temperatures.length < c.labels.length) {
							temperatures.unshift(null);
						}

						var ncds: Chart.ChartDataSets = {
							label: d.cds.label,
							backgroundColor: d.cds.backgroundColor,
							borderColor: d.cds.borderColor,
							fill: d.cds.fill,
							borderWidth: d.cds.borderWidth,
							lineTension: d.cds.lineTension,
							data: temperatures
						}
						return ncds;
					});

					createChart(c, concreteDataSet);
				});
			}
		});
	}));
});