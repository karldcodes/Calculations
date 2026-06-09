import type { FormField } from "../types/formField"
import { FormError } from "./formError"

export function FormField({ field, validationErrors }: FormField){
    return (
        <div id={field.id}>
            <label className="block text-sm font-medium text-slate-700 mb-1">{field.label}</label>
            
            {(validationErrors[field.name] ?? []).map(error => (
                <FormError error={error} />
            ))}

            <input
                type={field.type}
                name={field.name}
                {...field.metadata}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none"
            />
        </div>
    )
}