import React from "react";
import { Chart } from "react-chartjs-2";
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend, PointElement, LineElement} from 'chart.js';
import type { CityData } from './App';
import 'chartjs-adapter-moment';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend, PointElement, LineElement);

interface ChartComponentProps {
    data: CityData;
}


const ChartComponent: React.FC<ChartComponentProps> = ({ data }) => {

    // Extract relevant data from the JSON
    const labels = data.temperatures.map((item) => item.updateTimeStamp);
    const temperatures = data.temperatures.map((item) => item.temperature);

    // Get the maximum value
    const max = temperatures.reduce((a, b) => Math.max(a, b));
    // Get the minimum value
    const min = temperatures.reduce((a, b) => Math.min(a, b));

    const maxData: number[] = new Array(temperatures.length);
    maxData.fill(max);

    const minData: number[] = new Array(temperatures.length);
    minData.fill(min);


    // Chart data configuration
    const chartData = {
        labels: labels,
        datasets: [
            {
                label: "Max",
                backgroundColor: "rgba(97, 171, 64, 0.6)",
                borderColor: "rgba(97, 171, 64, 1)",
                borderWidth: 1,
                hoverBackgroundColor: "rgba(97, 171, 64, 0.8)",
                hoverBorderColor: "rgba(97, 171, 64, 1)",
                data: maxData,
                type: "line" as const,
                radius: 0,
                hitRadius: 5,
                order: 2
            },
            {
                label: "Min",
                backgroundColor: "rgba(0, 168, 255, 0.6)",
                borderColor: "rgba(0, 168, 255, 1)",
                borderWidth: 1,
                hoverBackgroundColor: "rgba(0, 168, 255, 0.8)",
                hoverBorderColor: "rgba(0, 168, 255, 1)",
                data: minData,
                type: "line" as const,
                radius: 0,
                hitRadius: 5,
                order: 1
            },
            {
                label: "Temperatures",
                backgroundColor: "rgba(255, 99, 132, 0.6)",
                borderColor: "rgba(255, 99, 132, 1)",
                borderWidth: 1,
                hoverBackgroundColor: "rgba(255, 99, 132, 0.8)",
                hoverBorderColor: "rgba(255, 99, 132, 1)",
                data: temperatures,
                order: 3
            }
        ]
    };

    const chartOptions = {
        responsive: true,
        parsing: {
            xAxisKey: 'updateTimeStamp',
            yAxisKey: 'temperature'
        },
        scales: {
            y: {
                beginAtZero: true
            }
        }
    };

    return (
        <div>
            <h3>{data.cityName}</h3>
            <Chart type='bar' data={chartData} options={chartOptions} />;
        </div>
    );
};

export default ChartComponent;