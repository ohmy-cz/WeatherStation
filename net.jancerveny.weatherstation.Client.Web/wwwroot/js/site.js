// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

jQuery(function ($) {
	var chart = null;
	var readingsSince = null;

	var getChartConfig = function (cb) {
		$.get('/data/GetChartConfig', function (r) {
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

	var getDataReadings = function (cb) {
		$.get('/data/GetReadings', { since: readingsSince }, function (r) {
			readingsSince = r.timestamp; //  Makes sure we only get the delta
			if (cb) {
				cb(r.readings);
			}
		});
	};

	var getChartData = function (cb) {
		if (!cb) {
			return;
		}
		getDataSources((dataSources) => getDataReadings((dataReadings) => {
			var ds = [];
			for (var i in dataSources) {
				var data = [];
				for (var j in dataReadings) {
					if (dataReadings[j].sourceId === dataSources[i].id) {
						data.push(dataReadings[j].temperature / 100);
					}
				}

				if (data.length === 0) {
					// this dataset has  no data to display, so let's omit this readout.
					continue;
				}

				ds.push({
					label: dataSources[i].name,
					data: data,
					backgroundColor: `rgba(${dataSources[i].color.r}, ${dataSources[i].color.g}, ${dataSources[i].color.b}, ${dataSources[i].color.a})`,
					borderColor: `rgba(${dataSources[i].color.r}, ${dataSources[i].color.g}, ${dataSources[i].color.b}, 1)`,
					fill: false,
					borderWidth: 3,
					lineTension: 0.1
				});
			}
			cb(ds);
		}));
	};

	// Initial render
	getChartConfig((chartConfig) => getChartData((datasets) => {
		var labels = [];
		for (var i in chartConfig) {
			labels.push(chartConfig[i].label);
		}
		if (labels.length === 0) {
			return;
		}
		var ctx = document.getElementById('myChart').getContext('2d');
		chart = new Chart(ctx, {
			type: 'line',
			data: {
				labels,
				datasets
			}
		});

		if (chart) {
			setInterval(function () {
				getChartData((delta) => {
					if (!delta) {
						return;
					}
					console.info(delta);
					//chart.data.datasets[0].data.pop();
					chart.data.datasets.forEach((dataset) => {
						// Rolling table - remove the oldest data when newly added data reach the ful lwidth of the chart.
						if (dataset.data.length >= labels.length) { 
							dataset.data.pop();
						}
						delta.forEach((d) => {
							if (dataset.label === d.label) {
								d.data.forEach((data) => {
									dataset.data.push(data);
								});
							}
						});
					});
					chart.update();
				});
			}, 60 * 1000);
		}
	}));
});