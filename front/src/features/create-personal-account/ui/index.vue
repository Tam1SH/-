<template>
	<Dialog v-model:visible="visible" modal header="Создать счёт" :style="{ width: '25rem' }">
		<!-- И это тоже форма... -->
		<div class="flex justify-between items-center">
			<span>Валюта счёта</span>
			<Select v-model="selected" :options="currencies" optionLabel="name" placeholder="Выберите валюту" class="w-full md:w-56" />

		</div>
		<Button @click="create" class="w-full mt-6">
			Создать
		</Button>
	</Dialog>
</template>
<script setup lang="ts">
import { Currency } from '@/shared/api/g/api'
import { useQueryAPI } from '@/shared/api/hooks'
import { useQueryClient } from '@tanstack/vue-query'
import { useToast } from 'primevue/usetoast'
import { ref, watchEffect } from 'vue'

const toast = useToast()
const queryClient = useQueryClient()


const { query: createQuery } = useQueryAPI({ enabled: false }).createPersonalAccount(() => ({
	currency: selected.value?.code!
}))

const create = async () => {
	
	const result = await createQuery.refetch()
	if(!result.error) {
		visible.value = false
		queryClient.invalidateQueries()
	}
	else {
		toast.add({ severity: 'error', detail: result.error.message, life: 3000 })
	}
}

const selected = ref<{ name: string, code: Currency | null} | null>(null);

const currencies = ref([
    { name: 'Рубли', code: Currency.NUMBER_0 },
    { name: 'y.e', code: Currency.NUMBER_1 },
]);

const props = defineProps<{
	visible: boolean
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

</script>