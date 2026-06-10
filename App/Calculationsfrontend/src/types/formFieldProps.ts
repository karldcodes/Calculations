import type { Field } from "./calculation";
import type { ValidationErrors } from "./validationErrors";


export type FormFieldProps = {
    field: Field;
    validationErrors: ValidationErrors;
}
