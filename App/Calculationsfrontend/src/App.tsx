import { useState } from 'react'
import useSWR from 'swr'
import { get } from "./clients/get"
import type { Calculation } from './types/calculation';

// wrapper to abstract and seperate api call logic
function getCalculations(){
    // use SWR so the calculations are cached for repeat calls
    const { data, error, isLoading } = useSWR<Calculation[]>(`/calculations`, get);

    return {
        calculations: data,
        isLoading,
        error: error
    }
}


function App() {
    const {calculations, isLoading, error} = getCalculations();
    const [selectedCalc, setSelectCalc] = useState<Calculation>();

    function findCaclulation(id: string) {
        setSelectCalc(calculations?.find(calc => calc.id === id));
    }

    async function formSubmit(formData: FormData) {

        try{
            // convert formData array into object
            const json: Record<string, string> = {};

            // convert every field value into a string as its dynamic this way the backend can handle any type conversion keeping the FE simple
            for (const [key, value] of formData.entries()) {
                if (key === "selectedFunction") continue;

                json[key] = value.toString();
            }

            // send to backend to get the value
            const API_URL = import.meta.env.VITE_API_URL;
            const response = await fetch(`${API_URL}/calculations/${selectedCalc?.name}`, {
                method: "POST",
                headers: {
                'Content-Type': 'application/json'
                },
                body: JSON.stringify(json)
            });

            if (!response.ok)
            {
                // error processing calc
            }

            const result = await response.json();

            console.log(result);

        }catch{

        }
    }

    if (error) {
        return (
            <div className="bg-slate-100 min-h-screen flex items-center justify-center p-6">
                <div>Unable to fetch data from backend!</div>
            </div>
        )
    }

    if (isLoading){
        return (
            <div className="bg-slate-100 min-h-screen flex items-center justify-center p-6">
                <div>Loading</div>
            </div>
        )
    }

    return (
        <div className="bg-slate-100 min-h-screen flex items-center justify-center p-6">
            <div className="w-full max-w-md bg-white rounded-xl shadow-lg p-6">
                <h1 className="text-2xl font-bold text-slate-800 mb-6">
                    Calculation Form
                </h1>
            <form action={formSubmit} className="space-y-4">
                
                <div>
                    <label htmlFor="calculation" className="block text-sm font-medium text-slate-700 mb-1">
                        Calculation Type
                    </label>
                    <select 
                        name="selectedFunction" 
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                        onChange={(event) => findCaclulation(event.target.value)}>
                            <option value="">Select a calculation</option>
                            {calculations?.map(calc =>
                                <option key={calc.id} value={ calc.id }>{calc.name}</option>
                            )}
                    </select>
                </div>

                {selectedCalc?.requestFields.map(field =>
                    <div id={field.id} key={ field.id }>
                        <label className="block text-sm font-medium text-slate-700 mb-1">{ field.name }</label>
                        <input 
                            type={field.type} 
                            name={field.name}
                            step="0.1"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                        />
                    </div>
                )}

                <button
                    type="submit"
                    className="w-full bg-blue-600 text-white py-2.5 rounded-lg font-medium hover:bg-blue-700 transition-colors"
                >
                    Calculate
                </button>

            </form>

            <div
                id="resultContainer"
                className="hidden mt-6 rounded-lg border border-slate-200 bg-slate-50 p-4"
            >
                <h2 className="text-sm font-medium text-slate-600 mb-1">Result</h2>
                <p id="result" className="text-2xl font-bold text-slate-800"></p>
            </div>

            </div>
        </div>
    )
}

export default App
