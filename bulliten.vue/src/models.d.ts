declare interface IUserAccount {
  id: number,
  username: string
}

declare interface IPost {
  id: number,
  author: IUserAccount,
  content: string,
  creationDate: Date
}
