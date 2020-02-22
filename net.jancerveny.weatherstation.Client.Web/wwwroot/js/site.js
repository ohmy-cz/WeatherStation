// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

jQuery(function ($) {
    $.get('/data/GetDataSources', function (r) {
        $.get('/data/GetReadings', function (r1) {

			var ds = [];
			for (var i in r) {
				var id = r[i].id;
				var data = [];
				for (var j in r1) {
					if (r1[j].sensorId === id) {
						data.push(r1[j].temperature/100);
					}
				}
				if (data.length === 0) {
					// this dataset has  no data to display, so let's omit this readout.
					continue;
				}
				ds.push({
					label: r[i].name,
					data: data,
					backgroundColor: `rgba(${r[i].color.r}, ${r[i].color.g}, ${r[i].color.b}, 0.2)`,
					borderColor: `rgba(${r[i].color.r}, ${r[i].color.g}, ${r[i].color.b}, ${r[i].color.a})`,
					fill: true,
					borderWidth: 3,
					lineTension: 0.1
				});
			}

			var ctx = document.getElementById('myChart').getContext('2d');
			new Chart(ctx, {
				type: 'line',
				data: {
					labels: ["Three hours ago", "Two hours ago", "A hour ago", "-30mins", "-15 mins", "-10 mins", "-5 mins", "Now"],
					datasets: ds
				}
			});
        });
	});
});