import { render } from 'vitest-browser-react'
import { expect, test } from 'vitest'
import {FormField} from "../../src/components/formField"
import type {Field} from "../../src/types/calculation"

test('counter button increments the count', async () => {
    
  const field: Field = {
    id: "123",
    name: "",
    type: "",
    label: "",
    required: false,
    metadata: {
      min: 0,
      max: 1,
      step: 0.1,
      placeholder: "",
      disabled: false,
      readOnly: false
    },
  };
  const validationErrors = {};

  const screen = await render(<FormField field={field} validationErrors={validationErrors} />);

  

  await screen.getByRole('button', { name: 'Increment' }).click()

  await expect.element(screen.getByText('Count is 2')).toBeVisible()
})