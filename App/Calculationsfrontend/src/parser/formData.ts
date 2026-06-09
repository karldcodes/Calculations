export function parseFormData(formData: FormData): Record<string, string>{
    const json: Record<string, string> = {};

    // convert every field value into a string as its dynamic this way the backend can handle any type conversion keeping the FE simple
    for (const [key, value] of formData.entries()) {
        if (key === "selectedFunction") continue;

        json[key] = value.toString();
    }
    return json;
}