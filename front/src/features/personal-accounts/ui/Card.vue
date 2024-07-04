<template>
	<div class="bg-slate-300 p-2 rounded-lg shadow-md flex items-center">
		<div class="flex flex-col gap-1">
			<span class="font-semibold">
				Счёт #{{ account.id }}
			</span>
		
			<div class="flex gap-1">
				<span>{{ account.amount }}</span>
				<span>{{ account.currency }}</span>
			</div>
		</div>
		<div class="ml-auto">
			<div class="cursor-pointer" @click="toggle">
				<MiOptionsHorizontal />
			</div>
			<Popover ref="pop">
				<div class="flex flex-col gap-2">
					<span class="cursor-pointer" @click="() => transaferVisible = true">
						Перевести
					</span>
					<div class="h-px w-full bg-slate-100" />
					<span class="cursor-pointer" @click="() => convertCurrencyVisible = true">
						Конвертировать валюту
					</span>
				</div>
			</Popover>
		</div>
		<Transfer v-model:visible="transaferVisible" :selected-account="account.id!" />
		<ConvertCurrency v-model:visible="convertCurrencyVisible" :account="account" />
	</div>
</template>

<script setup lang="ts">
import Transfer from '@/features/transafer/ui/index.vue'
import ConvertCurrency from '@/features/convert-currency/ui/index.vue'

import { type PersonalAccount } from '@/shared/api/g/api'
import MiOptionsHorizontal from '@/shared/icons/Options.vue'
import { type PopoverMethods } from 'primevue/popover'
import { ref } from 'vue';

const convertCurrencyVisible = ref(false)
const transaferVisible = ref(false)

const pop = ref<PopoverMethods>(null!)

const toggle = (event: MouseEvent) => {
    pop.value.toggle(event)
}

const { account } = defineProps<{ account: PersonalAccount }>()

</script>