import type { FormError } from "../types/formError"

export function FormError({error}: FormError){
    return (
        <p key={error} className="text-red-500 py-2">
            {error}
        </p>
    )
}