import store from "@/store/index";

const baseUrl: string = process.env.VUE_APP_API_URL;

function getAuthHeader(): Headers {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const token = (store.state as any).auth.token || "";
  const headers = new Headers();

  headers.append("Authorization", token);

  return headers;
}

function fetchRequest(method: string, route: string, options: RequestInit) {
  return fetch(`${baseUrl}${route}`, {
    ...options,
    method,
    mode: "cors"
  });
}

async function getError(response: Response): Promise<Error> {
  return new Error((await response.json()).message);
}

export async function createAccount(username: string, password: string): Promise<AuthResponse> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  const response = await fetchRequest("POST", "/user/create", {
    body: form
  });

  if (!response.ok)
    throw await getError(response);

  return response.json();
}

export async function login(username: string, password: string): Promise<AuthResponse> {
  const form = new FormData();
  form.append("username", username);
  form.append("password", password);

  const response = await fetchRequest("POST", "/user/login", {
    body: form
  });

  if (!response.ok)
    throw await getError(response);

  return response.json();
}

export async function createPost(content: string): Promise<void> {
  const form = new FormData();
  form.append("content", content);

  const response = await fetchRequest("POST", "/post/create", {
    body: form,
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);
}

export async function getPublicFeed(username: string): Promise<Post[]> {
  const response = await fetchRequest("GET", `/post/feed/public?username=${username}`, {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);

  return (await response.json()).posts;
}

export async function getPersonalFeed(): Promise<Post[]> {
  const response = await fetchRequest("GET", "/post/feed/personal", {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);

  return (await response.json()).posts;
}

export async function getUserProfile(username: string): Promise<UserProfile> {
  const response = await fetchRequest("GET", `/user/profile?username=${username}`, {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);

  return await response.json();
}

export async function followUser(username: string): Promise<void> {
  const response = await fetchRequest("POST", `/user/follow?username=${username}`, {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);
}

export async function unfollowUser(username: string): Promise<void> {
  const response = await fetchRequest("DELETE", `/user/unfollow?username=${username}`, {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);
}

export async function likePost(postId: number): Promise<void> {
  const response = await fetchRequest("POST", `/post/like?postId=${postId}`, {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);
}

export async function unlikePost(postId: number): Promise<void> {
  const response = await fetchRequest("DELETE", `/post/like/remove?postId=${postId}`, {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);
}

export async function repost(postId: number): Promise<void> {
  const response = await fetchRequest("POST", `/post/repost?postId=${postId}`, {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);
}

export async function unRepost(postId: number): Promise<void> {
  const response = await fetchRequest("DELETE", `/post/repost/remove?postId=${postId}`, {
    headers: getAuthHeader()
  });

  if (!response.ok)
    throw await getError(response);
}

