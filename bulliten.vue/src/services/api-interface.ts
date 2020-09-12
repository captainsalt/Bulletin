import store from "@/store/index";

const baseUrl: string = process.env.VUE_APP_API_URL;

function getAuthHeader(): Headers {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const token = (store.state as any).auth.token || "";
  const headers = new Headers();

  headers.append("Authorization", token);

  return headers;
}

async function getErrorMesssage(response: Response): Promise<string> {
  return (await response.json()).message;
}

export async function createAccount(username: string, password: string): Promise<AuthResponse> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  const response = await fetch(`${baseUrl}/api/user/create`, {
    method: "POST",
    mode: "cors",
    body: form
  });

  if (!response.ok)
    throw Error(await getErrorMesssage(response));

  return response.json();
}

export async function login(username: string, password: string): Promise<AuthResponse> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  const response = await fetch(`${baseUrl}/api/user/login`, {
    method: "POST",
    mode: "cors",
    body: form
  });

  if (!response.ok)
    throw Error(await getErrorMesssage(response));

  return response.json();
}

export function createPost(content: string): Promise<Response> {
  const form = new FormData();

  form.append("content", content);

  return fetch(`${baseUrl}/api/post/create`, {
    method: "POST",
    mode: "cors",
    body: form,
    headers: getAuthHeader()
  });
}

export async function getPublicFeed(username: string): Promise<Post[]> {
  const response = await fetch(`${baseUrl}/api/post/feed/public?username=${username}`, {
    method: "GET",
    mode: "cors",
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw Error(await getErrorMesssage(response));

  return (await response.json()).posts;
}

export async function getPersonalFeed(): Promise<Post[]> {
  const response = await fetch(`${baseUrl}/api/post/feed/personal`, {
    method: "GET",
    mode: "cors",
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw Error(await getErrorMesssage(response));

  return (await response.json()).posts;
}

export async function getUser(username: string): Promise<UserAccount> {
  const response = await fetch(`${baseUrl}/api/user?username=${username}`, {
    method: "GET",
    mode: "cors",
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw Error(await getErrorMesssage(response));

  return (await response.json()).user;
}

export async function followUser(username: string): Promise<void> {
  const response = await fetch(`${baseUrl}/api/user/follow?username=${username}`, {
    method: "POST",
    mode: "cors",
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw Error((await response.json()).message);
}

export async function getFollowInfo(username: string): Promise<{ followers: UserAccount[]; following: UserAccount[] }> {
  const response = await fetch(`${baseUrl}/api/user/followinfo?username=${username}`, {
    method: "GET",
    mode: "cors",
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw Error((await response.json()).message);

  return response.json();
}
