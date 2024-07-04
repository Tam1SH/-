<template>
	<Dialog v-model:visible="visible" modal header="Регистрация" :style="{ width: '25rem' }">
		<form @submit="onSubmit">
				
			<div class="flex items-center gap-4 mb-4">
				<Field name='login' v-slot="{ field }">
					<label for="login" class="font-semibold w-24">Логин</label>
					<div class='flex flex-col'>
						<InputText 
							id="login" 
							class="w-[235px]"
							v-bind="field" 
							v-model="login"
							:invalid="!!errors.login"
							autocomplete="username"
						/>
					</div>
				</Field>
			</div>

			<div class="flex items-center gap-4 mb-4">
				<Field name="password" v-slot="{ handleChange, handleBlur }">
					<label for="password" class="font-semibold w-24">Password</label>
					<div class='flex flex-col'>
						<Password 
							@blur="handleBlur"
							@change="handleChange"
							v-model="password"
							toggleMask 
							:feedback="false" 
							:invalid="!!errors.password"
							:input-props="{	
								autocomplete: 'on', 
								id: 'password',
								name: 'password'
							}"
						/>
					</div>
				</Field>
			</div>

			<div class="flex items-center gap-4 mb-8">
				<Field name="confirmPassword" v-slot="{ handleChange, handleBlur }">
					<label for="Confirm password" class="font-semibold w-24">Confirm password</label>
					<div>
						<div class='flex flex-col'>
							<Password
								@blur="handleBlur"
								@change="handleChange"
								v-model="confirmPassword"
								toggleMask 
								:feedback="false" 
								:invalid="!!errors.confirmPassword"
								:input-props="{	
									autocomplete: 'on', 
									id: 'Confirm password',
									name: 'confirmPassword'
								}"
							/>
						</div>
						<template v-if="errors.confirmPassword === 'Пароли не совпадают'">	
							<small class="ml-auto text-red-500">{{ errors.confirmPassword }}</small>
						</template>
					</div>

				</Field>
					

			</div>
			<div class="flex justify-center">
				<Button type="submit" label="Зарегистрироваться" />
			</div>
		</form>

		<template v-if="query.error.value">
			<div class="absolute -ml-5 mt-10 w-full bg-red-500 flex rounded-xl p-4">
				<span class="text-white font-semibold">
					{{ query.error.value.message }}
				</span>
			</div>
		</template>	
	</Dialog>
</template>
<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import { object, string } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'
import { useForm } from 'vee-validate'
import { Field } from 'vee-validate'
import { useQueryAPI } from '@/shared/api/hooks'
import { useToast } from 'primevue/usetoast'

const toast = useToast()

const props = defineProps<{
	visible: boolean
}>()

const emit = defineEmits<{
	'update:visible': [visible: boolean]
}>()

const [login, password, confirmPassword] = [ref(''), ref(''), ref('')]


const validationSchema = toTypedSchema(
	object({
		login: string({ required_error: 'Это поле обязательно' })
				.min(1, { message: 'Это поле обязательно' }),
		password: string({ required_error: 'Это поле обязательно' })
					.min(1, { message: 'Это поле обязательно' }),
		confirmPassword: string({ required_error: 'Это поле обязательно' })
							.min(1, { message: 'Это поле обязательно' })
							.refine((value) => value === password.value, {
								message: 'Пароли не совпадают',
							}),
	})
)

const { errors, handleSubmit } = useForm({ validationSchema })

const visible = ref(props.visible)

const { query } = useQueryAPI({ enabled: false }).register(() => ({
	login: login.value,
	password: password.value
}))

const onSubmit = handleSubmit(
	async () => {
		const result = await query.refetch()
		if(!result.error) {
			toast.add({ severity: 'info', summary: 'Info', detail: 'Регистрация прошла успешно', life: 30000 })
			visible.value = false
		}
	},
	({ errors }) => Object
						.keys(errors)
						.reverse()
						.forEach(err => document
											.getElementsByName(err)[0]
											?.focus())
)


watchEffect(() => {
  	visible.value = props.visible
})

watchEffect(() => {
  	emit('update:visible', visible.value)
})

</script>