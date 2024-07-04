<template>
	<header class="h-16 bg-slate-200 w-full sticky shadow-lg flex items-center">
		<div class="max-w-screen-xl w-full m-auto flex pl-4 pr-4">
			<div class="ml-auto flex gap-2">
				<template v-if="isUserLoggedIn">
					<Button severity="danger" @click="async () => {
						await logout.refetch()
						userStore.reset()
					}">
						<span class="text-white">
							Выйти
						</span>
					</Button>
				</template>
				<template v-else>
					<Button @click="() => loginVisible = true">
						<span class="text-white">
							Войти
						</span>
					</Button>
					<div class="w-px bg-slate-400"></div>
					<Button severity="info" @click="() => registerVisible = true">
						<span class="text-white">
							Зарегистрироваться
						</span>
					</Button>
				</template>
			</div>
		</div>
		<div>
			<Login v-model:visible="loginVisible" />
			<Register v-model:visible="registerVisible" />
		</div>
	</header>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import Login from '@/features/login/ui/index.vue'
import Register from '@/features/register/ui/index.vue'
import { useUserStore } from '@/app/userStore'
import { useQueryAPI } from '@/shared/api/hooks'

const loginVisible = ref(false)
const registerVisible = ref(false)

const userStore = useUserStore()

const { query: logout } = useQueryAPI({ enabled: false }).logout()

const isUserLoggedIn = computed(() => !!userStore.user)
</script>