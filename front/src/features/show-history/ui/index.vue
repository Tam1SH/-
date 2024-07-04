<template>
    <div class="flex flex-col gap-2 mt-2">
        <Button class="ml-auto flex gap-2 mb-4 items-center" @click="toggleFilters">
            <span>Фильтры</span>
            <OptionsIcon fill-color="white" />
        </Button>
        <Popover ref="filtersPanel">
            <div class="flex flex-col gap-2 p-4">
                <label>
                    Тип транзакции:
                    <Select v-model="filters.type" :options="transactionTypes" optionLabel="label" placeholder="Тип транзакции" class="w-full md:w-14rem" />
                </label>
                <div class="flex justify-between items-center">
                    <label>С даты:</label>
                    <InputText type="date" v-model="filters.fromDate" />
                </div>
                <div class="flex justify-between items-center">
                    <label>По дату:</label>
                    <InputText type="date" v-model="filters.toDate" />
                </div>
                <div class="flex justify-between items-center">
                    <label>Валюта:</label>
                    <InputText type="text" v-model="filters.currency" />
                </div>
                <div class="flex justify-between items-center">
                    <label>ID счёта:</label>
                    <InputText type="number" v-model="filters.accountId" />
                </div>
                <Button @click="applyFilters">Применить фильтры</Button>
            </div>
        </Popover>
        <template v-for="item in filteredHistory">
            <div 
                class="bg-slate-100 p-2 rounded-lg flex flex-col"
                :class="{
                    ['bg-green-300']: isIncomingTransfer(item.receiverUserId)
                }"
            >
                <small class="ml-auto">
                    {{ formatDate(item.transactionDate!) }}
                </small>
                <template v-if="isIncomingTransfer(item.receiverUserId)">
                    <span>+ {{ item.amount }} {{ item.currency }} на счёт #{{ item.toAccountId }} от пользователя №{{ item.senderUserId }}</span>
                    <small>Входящие переводы</small>
                </template>
                <template v-else>
                    <span>- {{ item.amount }} {{ item.currency }} со счёта #{{ item.fromAccountId }} пользователю №{{ item.receiverUserId }}</span>
                    <small class="mt-2">Исходящие переводы</small>
                </template>
            </div>
        </template>
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useQueryAPI } from '@/shared/api/hooks'
import { useUserStore } from '@/app/userStore'
import OptionsIcon from '@/shared/icons/Options.vue'

const { user } = useUserStore()

const isIncomingTransfer = (receiverUserId?: number) => receiverUserId === +user?.id!

const filters = reactive({
    fromDate: null as string | null,
    toDate: null as string | null,
    currency: null as string | null,
    accountId: null as number | null,
    type: null as { value: string, label: string } | null
})

const transactionTypes = [
    { value: 'all', label: 'Все' },
    { value: 'incoming', label: 'Входящие' },
    { value: 'outgoing', label: 'Исходящие' }
]

const { query: historyQuery } = useQueryAPI().getTransactionHistory(() => ({
    fromDate: filters.fromDate ? new Date(filters.fromDate!) : undefined,
    toDate: filters.toDate ? new Date(filters.toDate) : undefined,
    accountId: filters.accountId ?? undefined,
    currency: filters.currency ?? undefined
}))

onMounted(() => {
    historyQuery.refetch()
})

const filteredHistory = computed(() => {
    if (!historyQuery.data.value) return []
    return historyQuery.data.value.filter(item => {
        if (filters.type?.value === 'all') return true
        if (filters.type?.value === 'incoming') return isIncomingTransfer(item.receiverUserId)
        if (filters.type?.value === 'outgoing') return !isIncomingTransfer(item.receiverUserId)
        return true
    })
})

const applyFilters = () => {
    historyQuery.refetch()
    filtersPanel.value.hide()
}

const formatDate = (date: Date): string => {
    const d = new Date(date)
    const day = String(d.getDate()).padStart(2, '0')
    const month = String(d.getMonth() + 1).padStart(2, '0')
    const year = String(d.getFullYear()).slice(-2)
    return `${day}.${month}.${year}`
}

const filtersPanel = ref()

const toggleFilters = (event: Event) => {
    filtersPanel.value.toggle(event)
}
</script>