<template>
	<Header/>
	<Toast position="bottom-center" />
	<div class="w-full flex" style="height: calc(100vh - 64px)">
		<template v-if="userStore.user">
			<Profile/>
		</template>
		<template v-else>
			<div class="m-auto font-semibold text-2xl">
				Войдите в систему.
			</div>
		</template>
	</div>
</template>
<script setup lang="ts">
import Header from '@/widget/Header/ui/index.vue'
import Profile from '@/widget/Profile/ui/index.vue'
import { useUserStore } from './app/userStore'
import { onMounted } from 'vue'
import { useFactoryApi } from './shared/api/hooks'
import { AuthenticateApi } from './shared/api/g/api'

const userStore = useUserStore()

onMounted(async () => {
	if(!userStore.token) {
		const api = useFactoryApi().create(AuthenticateApi)
		const { accessToken } = await api.refreshToken()
		userStore.setToken(accessToken!)
	}
})


</script>