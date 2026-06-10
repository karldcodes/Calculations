import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import {FormField} from "../../src/components/formField";
import type { Field } from "../../src/types/calculation";
import type { ValidationErrors } from "../../src/types/validationErrors";

describe("FormField", () => {
  it("renders dynamic properties on the input", async () => {
    const field: Field = {
        id: "1",
        name: "input1",
        label: "Input 1",
        type: "number",
        metadata: {
            min: 0,
            max: 1,
            step: 0.1,
            required: true
        }
    };

    const validationErrors: ValidationErrors = {};

    render(<FormField field={field} validationErrors={validationErrors} />);

    const input = await screen.findByLabelText(/Input 1/i);


    expect(input).toBeInTheDocument();
    expect(input).toHaveAttribute("type", "number");
    expect(input).toHaveAttribute("type", "number");
    expect(input).toHaveAttribute("required");
    expect(input).toHaveAttribute("min", "0");
    expect(input).toHaveAttribute("max", "1");
  });

  it("renders validation errors", async () => {
    const field: Field = {
        id: "1",
        name: "input1",
        label: "Input 1",
        type: "number",
        metadata: {
            min: 0,
            max: 1,
            step: 0.1,
            required: true
        }
    };

    const validationErrors: ValidationErrors = {
        "input1": ["A validation error"]
    };

    render(<FormField field={field} validationErrors={validationErrors} />);

    expect(screen.getByText("A validation error")).toBeInTheDocument();
  });
});