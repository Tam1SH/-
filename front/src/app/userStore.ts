import { User } from "@/shared/entities/User"
import parseJwt from "@/shared/libs/parseJwt"
import { defineStore } from "pinia"
import { ref } from "vue"

export const useUserStore = defineStore('user', () => {

	const user = ref<User | null>(null)

	const token = ref<string | null>(null)

	const setToken = (token_: string) => {
		token.value = token_
		user.value = parseJwt(token.value) as User
	}

	const reset = () => {
		token.value = null
		user.value = null
	}

	return {
		user, token, 
		setToken, reset
	}
})