<template>
	<div class="flex flex-col gap-2 mt-2">
		<template v-for="account in accounts">
			<Card :account="account"/>
		</template>
	</div>
	<template v-if="accounts?.length! < 5">
		<div class="mt-4">
			<Button @click="() => createPersonalAccountVisible = true">Создать счёт</Button>
		</div>
	</template>
	<CreatePersonalAccount v-model:visible="createPersonalAccountVisible"/>
</template>
<script setup lang="ts">
import type { PersonalAccount } from '@/shared/api/g/api'
import CreatePersonalAccount from '@/features/create-personal-account/ui/index.vue'
import Card from './Card.vue'
import { ref } from 'vue';

const createPersonalAccountVisible = ref(false)

const { accounts } = defineProps<{ accounts : PersonalAccount[] | undefined }>()
</script>