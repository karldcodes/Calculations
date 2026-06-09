export type Calculation = {
    id : string;
    name: string;
    requestFields: Field[];
    responseFields: Field[];
}

export type Field = {
    id: string;
    name: string;
    label: string;
    type: string;
    required: boolean;
    metadata?: FieldMetadata;
}

type FieldMetadata = {
  min?: number;
  max?: number;
  step?: number | string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  readOnly?: boolean;
};