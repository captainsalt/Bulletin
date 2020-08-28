import axios from "axios";

const baseUrl: string = process.env.VUE_APP_API_URL;

export async function createAccount(username: string, password: string): Promise<string> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  const response = await fetch(baseUrl + "/create", {
    method: "POST",
    mode: "cors",
    body: form
  });

  if (response.ok)
    return response.text();

  throw Error("Error creating account");
}
