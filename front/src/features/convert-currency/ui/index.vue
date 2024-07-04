<template>
	<Dialog v-model:visible="visible" modal header="Конвертировать валюту" :style="{ width: '25rem' }">
		<div class="flex justify-between items-center">
			<span>Выбранный счёт</span>
			<span>#{{ account.id }}</span>
		</div>
		<div class="flex justify-between items-center mt-4">
			<span>Целевая валюта</span>
			<Select v-model="selectedCurrency" :options="currencies" optionLabel="name" placeholder="Выберите валюту" class="w-full md:w-56" />
		</div>
		<Button @click="convert" class="w-full mt-6">
			Конвертировать
		</Button>
	</Dialog>
</template>
  
<script setup lang="ts">
import { Currency, type PersonalAccount } from '@/shared/api/g/api'
import { useQueryAPI } from '@/shared/api/hooks'
import { useQueryClient } from '@tanstack/vue-query'
import { useToast } from 'primevue/usetoast'
import { ref, watchEffect } from 'vue'

const toast = useToast()
const queryClient = useQueryClient()
  
const { query: convertQuery } = useQueryAPI({ enabled: false }).convertCurrency(() => ({
	accountId: props.account.id,
	targetCurrency: selectedCurrency.value?.code!
}))
  
const convert = async () => {
	const result = await convertQuery.refetch()
	if (!result.error) {
	  	toast.add({ severity: 'success', detail: `Сумма в новой валюте: ${result.data}`, life: 3000 })
	  	visible.value = false
	  	queryClient.invalidateQueries()
	} else {
	  	toast.add({ severity: 'error', detail: result.error.message, life: 3000 })
	}
}
  
const selectedCurrency = ref<{ name: string, code: Currency } | null>(null)
  
const props = defineProps<{
	visible: boolean
	account: PersonalAccount
}>()
  
const emit = defineEmits<{
	'update:visible': [visible: boolean]
}>()
  
const visible = ref(props.visible)
  
watchEffect(() => {
	visible.value = props.visible
})
  
watchEffect(() => {
	emit('update:visible', visible.value)
})
  
const currencies = ref([
	{ name: 'Рубли', code: Currency.NUMBER_0 },
	{ name: 'y.e', code: Currency.NUMBER_1 },
])
</script>