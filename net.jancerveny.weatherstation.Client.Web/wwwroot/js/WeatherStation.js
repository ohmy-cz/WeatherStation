var ChartTypeEnum;
(function (ChartTypeEnum) {
    ChartTypeEnum[ChartTypeEnum["RealTime"] = 0] = "RealTime";
    ChartTypeEnum[ChartTypeEnum["Monthly"] = 1] = "Monthly";
    ChartTypeEnum[ChartTypeEnum["Weekly"] = 2] = "Weekly";
    ChartTypeEnum[ChartTypeEnum["Last7Days"] = 3] = "Last7Days";
    ChartTypeEnum[ChartTypeEnum["Annual"] = 4] = "Annual";
})(ChartTypeEnum || (ChartTypeEnum = {}));
jQuery(function ($) {
    var readingsSince = null;
    var dataSources1 = [];
    var getChartsConfig = function (cb) {
        $.get('/data/GetChartsConfig', function (r) {
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
        $.get('/data/GetReadings', { since: readingsSince }, function (r) {
            readingsSince = r.timestamp;
            if (cb) {
                cb(r.readings);
            }
        });
    };
    var getAggregationReadouts = function (cb) {
        $.get('/data/GetAggregations', function (r) {
            if (cb) {
                cb(r);
            }
        });
    };
    var getChartsDataSources = function (cb) {
        if (!cb) {
            return;
        }
        getDataSources(function (dataSources) {
            for (var i in dataSources) {
                dataSources1.push({
                    id: dataSources[i].id,
                    cds: {
                        label: dataSources[i].name,
                        backgroundColor: "rgba(" + dataSources[i].color.r + ", " + dataSources[i].color.g + ", " + dataSources[i].color.b + ", " + dataSources[i].color.a + ")",
                        borderColor: "rgba(" + dataSources[i].color.r + ", " + dataSources[i].color.g + ", " + dataSources[i].color.b + ", 1)",
                        fill: false,
                        borderWidth: 3,
                        lineTension: 0.1
                    }
                });
            }
            cb(dataSources1);
        });
    };
    var createChart = function (chartConfig, datasets) {
        var labels = chartConfig.labels.map(function (l) { return l.label; });
        if (labels.length === 0) {
            return;
        }
        var canvas = document.getElementById(ChartTypeEnum[chartConfig.chartType]);
        var ctx = canvas.getContext('2d');
        return new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: datasets
            },
            options: {
                title: {
                    display: true,
                    text: chartConfig.name
                }
            }
        });
    };
    getChartsConfig(function (chartConfig) { return getChartsDataSources(function (ds) {
        chartConfig.forEach(function (c) {
            if (c.chartType === ChartTypeEnum.RealTime) {
                getDataReadouts(function (dr) {
                    var concreteDataSet = ds.map(function (d) {
                        var temperatures = dr.filter(function (dr1) { return dr1.sourceId == d.id; }).map(function (dr1) { return dr1.temperature / 100; });
                        while (temperatures.length < c.labels.length) {
                            temperatures.unshift(null);
                        }
                        var ncds = {
                            label: d.cds.label,
                            backgroundColor: d.cds.backgroundColor,
                            borderColor: d.cds.borderColor,
                            fill: d.cds.fill,
                            borderWidth: d.cds.borderWidth,
                            lineTension: d.cds.lineTension,
                            data: temperatures
                        };
                        return ncds;
                    });
                    var realtimeChart = createChart(c, concreteDataSet);
                    setInterval(function () {
                        getDataReadouts(function (delta) {
                            if (!delta || delta.length === 0) {
                                return;
                            }
                            realtimeChart.data.datasets.forEach(function (dataset) {
                                dataset.data.splice(0, 1);
                            });
                            realtimeChart.data.datasets.forEach(function (dataset) {
                                var dsf = dataSources1.filter(function (x) { return x.cds.label == dataset.label; });
                                if (dsf.length == 0) {
                                    return;
                                }
                                var datasetId = dsf[0].id;
                                var df = delta.filter(function (x) { return x.sourceId == datasetId; });
                                var lastKnownValue = typeof dataset.data[dataset.data.length - 1] === "number" ? dataset.data[dataset.data.length - 1] : null;
                                var temp = df.length > 0 ? (df[0].temperature / 100) : lastKnownValue;
                                console.log("Adding column to: " + dataset.label + " temp: " + temp);
                                dataset.data.push(temp);
                            });
                            realtimeChart.update();
                        });
                    }, 10 * 1000);
                });
            }
            if (c.chartType === ChartTypeEnum.Last7Days) {
                getAggregationReadouts(function (dr) {
                    var concreteDataSet = ds.map(function (d) {
                        var temperatures = dr.filter(function (dr1) { return dr1.sourceId == d.id; }).map(function (dr1) { return dr1.temperature / 100; });
                        while (temperatures.length < c.labels.length) {
                            temperatures.unshift(null);
                        }
                        var ncds = {
                            label: d.cds.label,
                            backgroundColor: d.cds.backgroundColor,
                            borderColor: d.cds.borderColor,
                            fill: d.cds.fill,
                            borderWidth: d.cds.borderWidth,
                            lineTension: d.cds.lineTension,
                            data: temperatures
                        };
                        return ncds;
                    });
                    createChart(c, concreteDataSet);
                });
            }
        });
    }); });
});
//# sourceMappingURL=WeatherStation.js.map