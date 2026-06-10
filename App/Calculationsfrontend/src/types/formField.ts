import type { Field } from "./calculation";
import type { ValidationErrors } from "./validationErrors";


export type FormField = {
    field: Field;
    validationErrors: ValidationErrors;
}
