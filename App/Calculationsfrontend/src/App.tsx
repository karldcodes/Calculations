import { useState } from 'react';
import { parseFormData } from './parser/formData';
import { useCalculations } from './datahooks/useCalculations';
import type { ValidationErrors } from './types/ValidationErrors';
import { FormField } from './components/formField';
import { FormError } from './components/formError';

function App() {
    const { calculations, isLoading, error } = useCalculations();
    const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});
    const [selected, setSelected] = useState("");
    const [calculationResult, setCalculationResult] = useState<number | null>(null);

    const selectedCalc = calculations.find(calc => calc.id === selected);

    async function formSubmit(formData: FormData) {
        setValidationErrors({});
        setCalculationResult(null);

        if (!selectedCalc) {
            setValidationErrors({
                _generic: ["Please select a calculation."]
            });
            return;
        }

        // Run submission logic
        try {
            const json = parseFormData(formData);
            const API_URL = import.meta.env.VITE_API_URL;
            const response = await fetch(
                `${API_URL}/calculations/${encodeURIComponent(selectedCalc.name)}`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(json)
                }
            );

            if (response.status === 400) {
                const problem = await response.json();
                setValidationErrors(problem.errors ?? {});
                return;
            }

            if (!response.ok) {
                setValidationErrors({
                    _generic: ["Something went wrong. Please try again."]
                });
                return;
            }

            const result = await response.json();
            setCalculationResult(result.value);

        } catch {
            setValidationErrors({
                _generic: ["Unable to connect to the backend."]
            });
        }
    }

    if (error) {
        return (
            <div className="bg-slate-100 min-h-screen flex items-center justify-center p-6">
                <div>Unable to fetch data from backend!</div>
            </div>
        )
    }

    if (isLoading) {
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
                <form onSubmit={(event) => {
                    event.preventDefault();
                    formSubmit(new FormData(event.currentTarget));
                }}
                    className="space-y-4">

                    <div>
                        <label htmlFor="calculation" className="block text-sm font-medium text-slate-700 mb-1">
                            Calculation Type
                        </label>
                        
                        {(validationErrors["_generic"] ?? []).map(error => (
                            <FormError key={error} error={error} />
                        ))}

                        <select
                            id="calculation"
                            name="selectedFunction"
                            value={selected}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
                            onChange={(event) => {
                                setSelected(event.target.value);
                                setValidationErrors({});
                                setCalculationResult(null);
                            }}>
                            <option value="">Select a calculation</option>
                            {calculations?.map(calc =>
                                <option key={calc.id} value={calc.id}>{calc.name}</option>
                            )}
                        </select>
                    </div>

                    {selectedCalc?.requestFields.map(field =>
                        <FormField key={field.id} field={field} validationErrors={validationErrors} />
                    )}

                    <button
                        type="submit"
                        className="w-full bg-blue-600 text-white py-2.5 rounded-lg font-medium hover:bg-blue-700 transition-colors"
                    >
                        Calculate
                    </button>

                </form>

                {calculationResult !== null && (
                    <div
                        id="resultContainer"
                        className="mt-6 rounded-lg border border-slate-200 bg-slate-50 p-4"
                    >
                        <h2 className="text-sm font-medium text-slate-600 mb-1">Result</h2>
                        <p id="result" className="text-2xl font-bold text-slate-800">{calculationResult}</p>
                    </div>
                )}

            </div>
        </div>
    )
}

export default App
