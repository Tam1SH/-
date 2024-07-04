<template>
	<div class="w-[600px] bg-slate-200 rounded-lg shadow-md p-4 flex flex-col">
		<span class="font-bold font-xl">
			{{ label }}
		</span>
		<slot />
	</div>
</template>
<script setup lang="ts">
const { label } = defineProps<{
	label: string
}>()
</script>