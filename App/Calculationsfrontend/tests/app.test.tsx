import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import App from "../src/App";

// mock the get call 
vi.mock("../src/clients/get", () => ({
  get: vi.fn(() =>
    Promise.resolve([
      {
        id: "either",
        name: "either",
        requestFields: [
          {
            id: "ProbabilityA",
            name: "ProbabilityA",
            label: "Probability A",
            type: "number",
            metadata: {
              min: "0",
              max: "1",
              step: "any",
              required: true,
            },
          },
          {
            id: "ProbabilityB",
            name: "ProbabilityB",
            label: "Probability B",
            type: "number",
            metadata: {
              min: "0",
              max: "1",
              step: "any",
              required: true,
            },
          },
        ],
      },
    ])
  ),
}));

describe("App", () => {
  it("renders dynamic fields when a calculation is selected", async () => {
    const user = userEvent.setup();

    render(<App />);

    const select = await screen.findByLabelText(/calculation type/i);

    await user.selectOptions(select, "either");


    expect(screen.getByLabelText(/probability a/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/probability b/i)).toBeInTheDocument();
  });

  it("Renders the result of a calculation", async () => {
    // mock the submit call to the api
    vi.stubGlobal(
        "fetch",
        vi.fn(() =>
        Promise.resolve({
            ok: true,
            status: 200,
            json: () => Promise.resolve({ value: 0.75 }),
        })
        )
    );

    const user = userEvent.setup();

    render(<App />);

    await user.selectOptions(
        await screen.findByLabelText(/calculation type/i),
        "either"
    );

    // perform user actions and click calculate
    await user.type(screen.getByLabelText(/probability a/i), "0.5");
    await user.type(screen.getByLabelText(/probability b/i), "0.5");
    await user.click(screen.getByRole("button", { name: /calculate/i }));


    expect(await screen.findByText("0.75")).toBeInTheDocument();

    // screen.debug(); display html in console
  });
});