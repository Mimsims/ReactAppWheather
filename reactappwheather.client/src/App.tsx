import { useEffect, useState } from 'react';
import './App.css';
import ChartComponent from "./ChartComponent";

export interface CityData {
    cityName: string;
    temperatures: [{
        temperature: number;
        updateTimeStamp: string;
    }]
}

interface TemperaturesData {
    countryName: string;
    cityNames: CityData[];
}

const APIurl = 'https://localhost:7226/weatherforecast/temperatures';

function App() {

    useEffect(() => {
        populateTemperaturesData();
    }, []);

    const [temperatures, setTemperatures] = useState<TemperaturesData[]>();

    const contents = temperatures === undefined
        ? <p>
            <em>Loading... </em>
        </p>
        : <div>
            {temperatures.map(country =>
                <div>
                    <h2>{country.countryName} temperatures</h2>
                    {country.cityNames.map(city =>
                        <ChartComponent data={city} />
                    )}
                </div>
            )}           
        </div>;

    return (
        <div>
            <h1 id="tabelTemperatures">Weather temperatures:</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );

    async function populateTemperaturesData() {
        const response = await fetch(APIurl);
        const data = await response.json();
        setTemperatures(data);
    }

}

export default App;