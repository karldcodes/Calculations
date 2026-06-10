import { describe, expect, test } from 'vitest'
import {parseFormData} from "../../src/parser/formData"

describe('Form data parser', () => {
  test('Doesnt include selectedFunction', () => {
    const formData = new FormData();
    formData.append("selectedFunction", "Not me");
    formData.append("field1", "value1");


    const actual = parseFormData(formData);


    expect(actual).toEqual({field1: "value1"})
  })
})
