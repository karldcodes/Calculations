import type { Field } from "./calculation";
import type { ValidationErrors } from "./ValidationErrors";


export type FormField = {
    field: Field;
    validationErrors: ValidationErrors;
}
