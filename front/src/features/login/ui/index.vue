<template>
	<Dialog v-model:visible="visible" modal header="Войти" :style="{ width: '25rem' }">
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
			<div class="flex items-center gap-4 mb-8">
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
							aria-describedby="forgot_password"
							:input-props="{	
								autocomplete: 'on', 
								id: 'password',
								name: 'password'
							}"
						/>
					</div>

				</Field>
			</div>
			<div class="flex justify-center">
				<Button type="submit" label="Войти" />
			</div>
		</form>

		<template v-if="Auth.query.error.value">
			<div class="absolute -ml-5 mt-10 w-full bg-red-500 flex rounded-xl p-4">
				<span class="text-white font-semibold">
					{{ Auth.query.error.value.message }}
				</span>
			</div>
		</template>	
	</Dialog>
</template>
<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import { useLoginForUser } from '../model/login'
import { object, string } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'
import { useForm } from 'vee-validate'
import { Field } from 'vee-validate'

const props = defineProps<{
	visible: boolean
}>()

const emit = defineEmits<{
	'update:visible': [visible: boolean]
}>()

const [login, password] = [ref(''), ref('')]


const validationSchema = toTypedSchema(
	object({
		login: string({ required_error: 'Это поле обязательно' })
				.min(1, { message: 'Это поле обязательно' }),
		password: string({ required_error: 'Это поле обязательно' })
					.min(1, { message: 'Это поле обязательно' }),
	})
)

const { errors, handleSubmit } = useForm({ validationSchema })

const visible = ref(props.visible)


const { fn: signIn, hook: Auth } = useLoginForUser(() => ({
	login: login.value,
	password: password.value
})) 

const onSubmit = handleSubmit(
	async () => {
		await signIn()
		visible.value = false
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