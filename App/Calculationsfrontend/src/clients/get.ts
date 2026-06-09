const API_URL = import.meta.env.VITE_API_URL;

// Any future get requests from the same api can now use this
export async function get<T>(path: string): Promise<T> {
  const response = await fetch(`${API_URL}${path}`);

  if (!response.ok) {
    throw new Error(`HTTP ${response.status}`);
  }

  return response.json();
}