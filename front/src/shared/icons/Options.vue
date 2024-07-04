<template>
	<svg xmlns="http://www.w3.org/2000/svg" :style="{ color: fillColor }" width="1.5em" height="1.5em" viewBox="0 0 24 24">
	  <path :fill="fillColor" d="M12 14a2 2 0 1 0 0-4a2 2 0 0 0 0 4m-6 0a2 2 0 1 0 0-4a2 2 0 0 0 0 4m12 0a2 2 0 1 0 0-4a2 2 0 0 0 0 4" />
	</svg>
  </template>
  
<script setup lang="ts">
import { defineProps } from 'vue'
  
const props = defineProps({
	fillColor: {
		type: String,
		default: '#6363f3'
	}
})
</script>