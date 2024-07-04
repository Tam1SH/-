
import './assets/main.scss'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import PrimeVue from 'primevue/config'
import ToastService from 'primevue/toastservice'
import Aura from '@primevue/themes/aura'
import Button from 'primevue/button'
import Divider from 'primevue/divider'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import { VueQueryPlugin } from '@tanstack/vue-query'
import Toast from 'primevue/toast'
import Select from 'primevue/select'
import Popover from 'primevue/popover'
import FloatLabel from 'primevue/floatlabel'

const app = createApp(App)

app
	.use(createPinia())
	.use(VueQueryPlugin, {
		queryClientConfig: {
			defaultOptions: {
				queries: {
					retry: false
				}
			}
		}
	})
	.use(PrimeVue, {
		theme: {
			preset: Aura,
			options: {
				darkModeSelector: ''
			}
		}
	})
	.use(ToastService)

app
	.component('Button', Button)
	.component('Divider', Divider)
	.component('Dialog', Dialog)
	.component('InputText', InputText)
	.component('Password', Password)
	.component('Toast', Toast)
	.component('Popover', Popover)
	.component('FloatLabel', FloatLabel)
	.component('Select', Select)
app.mount('#app')
