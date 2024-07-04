export type UserData = {
	id : number
	login: string
}

export class User {

	id : number

	login : string

	constructor(userData : UserData) {

		this.id = userData.id
		this.login = userData.login
	}
	
	static toPlain(user?: User | null) {
		if(!user)
			return null
		return { ...user }
	}
}

	
