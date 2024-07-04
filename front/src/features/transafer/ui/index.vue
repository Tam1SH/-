<template>
	<Dialog v-model:visible="visible" modal header="Перевод" :style="{ width: '25rem' }">
		<!-- Это всё, правда, тоже форма... -->
		<div class="flex">
			<span class="font-semibold">Выберите ваш счёт</span>
			<template v-if="selectedSelfAccount !== null">
				<span 
					class="ml-auto cursor-pointer"
					@click="() => selectedSelfAccount = null"	
				>
					Сбросить
				</span>
			</template>
		</div>
		<div class="flex flex-col gap-2 mt-2" v-for="account in accounts">
			<div 
				:id="`${account.id}`" 
				:class="{
					['bg-slate-200']: selectedSelfAccount === account.id
				}"
				class="flex bg-slate-100 rounded-lg p-2 justify-between cursor-pointer"
				@click="() => {
					
					if(selectedSelfAccount == account.id) {
						selectedSelfAccount = null
					}
					else {
						selectedSelfAccount = account.id!
					}
				}"
			>
				<span> Счёт #{{ account.id }}</span>
				<span> {{ account.amount }} {{ account.currency }}</span>
			</div>
		</div>
		<div class="flex flex-col mt-6">
			<span class="font-semibold mb-6">Перевести кому</span>

			<FloatLabel>			
				<InputText @input="() => toAccountQuery.refetch()" class="w-full" id="UserId" v-model="toUserId" />
				<label for="UserId">ID пользователя</label>
			</FloatLabel>

			<template v-if="toAccountQuery.error.value">
				<span class="m-auto font-semibold mt-4">Такого пользователя не существует.</span>
			</template>
			<template v-else-if="toAccountQuery.data.value?.length === 0">
				<span class="m-auto font-semibold mt-4">У этого пользователя нет счетов.</span>
			</template>
			<template v-else>
				<span class="font-semibold mt-4">Счета пользователя </span>
				<div class="flex flex-col gap-2 mt-2" v-for="account in toAccountQuery.data.value">
					<div 
						:id="`${account.id}`" 
						:class="{
							['bg-slate-200']: selectedToAccount === account.id
						}"
						class="flex bg-slate-100 rounded-lg p-2 justify-between cursor-pointer"
						@click="() => {
						
							if(selectedToAccount == account.id) {
								selectedToAccount = null
							}
							else {
								selectedToAccount = account.id!
							}
						}"
					>
						<span> Счёт #{{ account.id }}</span>
						<span> {{ account.amount }} {{ account.currency }}</span>
					</div>
				</div>
			</template>
			<template v-if="selectedSelfAccount && selectedToAccount">
				<div class="mt-12 gap-4 flex flex-col">
					<FloatLabel>			
						<InputText 
							class="w-full" 
							id="UserId" 
							v-model="amountForTransfer"
							:invalid="+amountForTransfer! > accounts?.find(p => p.id === selectedSelfAccount)?.amount!"
						/>
						<label for="UserId">Сумма</label>
					</FloatLabel>

					<Button @click="transfer">
						Перевести
					</Button>
				</div>
			</template>
		</div>
	</Dialog>
</template>
<script setup lang="ts">
import { useUserStore } from '@/app/userStore'
import { useQueryAPI } from '@/shared/api/hooks'
import { useQueryClient } from '@tanstack/vue-query';
import { useToast } from 'primevue/usetoast';
import { ref, watchEffect } from 'vue'


const props = defineProps<{
	visible: boolean
	selectedAccount: number
}>()

const { user } = useUserStore()

const toast = useToast()
const queryClient = useQueryClient()

const toUserId = ref<string | null>(null)

const amountForTransfer = ref<string | null>(null)

const selectedSelfAccount = ref<number | null>(props.selectedAccount)

const selectedToAccount = ref<number | null>(null)

const { query: { data: accounts }} = useQueryAPI().getPersonalAccounts(user!.id)

const { 
	query: toAccountQuery, 
	remove: removeToAccountQuery 
} = useQueryAPI({ enabled: false, formatName: 'toAccountQuery' })
		.getPersonalAccounts(() => parseInt(toUserId.value!))

const { query: transaferQuery, remove: removeTransfer } = useQueryAPI({ enabled: false }).transferFunds(() => {
	return {
		amount: +amountForTransfer.value!,
		fromAccountId: selectedSelfAccount.value!,
		toUserId: +toUserId.value!,
		toAccountId: selectedToAccount.value!
	}
})


const emit = defineEmits<{
	'update:visible': [visible: boolean]
}>()

const visible = ref(props.visible)


const transfer = async () => {
	
	const result = await transaferQuery.refetch()

	if (!result.error) {
		toast.add({ severity: 'success', detail: 'Перевод произведён.', life: 3000 })

		visible.value = false

		queryClient.invalidateQueries()
		removeToAccountQuery()
		removeTransfer()
	}
	else {
		toast.add({ severity: 'error', detail: result.error.message, life: 3000 })
	}

}

watchEffect(() => {
  	visible.value = props.visible
})

watchEffect(() => {
	emit('update:visible', visible.value)
})

</script>