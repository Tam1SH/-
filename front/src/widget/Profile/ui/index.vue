<template>
	<div class="m-auto mt-12 flex flex-col gap-4">
		<Card label="Основная информация">
			<span>
				Логин: {{ user.login }}
			</span>
		</Card>
		<Card label="Лицевой счёт">
			<PersonalAccounts :accounts="accounts"/>
		</Card>
		<Card label="История переводов">
			<TransactionHistory/>
		</Card>
	</div>

</template>
<script setup lang="ts">

import { useQueryAPI } from '@/shared/api/hooks'
import Card from './Card.vue'
import { useUserStore } from '@/app/userStore'
import PersonalAccounts from '@/features/personal-accounts/ui/index.vue'
import TransactionHistory from '@/features/show-history/ui/index.vue'

const user = useUserStore().user!

const { query: { data: accounts }} = useQueryAPI().getPersonalAccounts(user.id)

</script>