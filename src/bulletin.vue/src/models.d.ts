declare interface UserAccount {
  id: number;
  username: string;
  creationDate: string;
}

declare interface Post {
  id: number;
  author: UserAccount;
  content: string;
  creationDate: string;
  likes: number;
  likeStatus: boolean;
  rePosts: number;
  rePostStatus: boolean;
}

declare interface UserProfile {
  user: UserAccount;
  followerCount: number;
  followingCount: number;
  isFollowing: boolean;
}

declare interface AuthResponse {
  token: string;
  user: UserAccount;
}
