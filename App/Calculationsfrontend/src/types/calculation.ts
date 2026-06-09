export interface Calculation {
    id : string,
    name: string,
    requestFields: Field[],
    responseFields: Field[]
}

export interface Field {
    id: string,
    name: string,
    type: string,
    required: boolean
}

