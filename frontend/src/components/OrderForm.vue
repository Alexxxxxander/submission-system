<script setup>
import { ref } from 'vue';
import { api } from '../services/api.js';
import { ApiError } from '../services/httpClient.js';
import { useFormValidation } from '../composables/useFormValidation.js';
import { useToast } from '../composables/useToast.js';

const emit = defineEmits(['submitted']);

const { success, error: showError } = useToast();

const formData = ref({
  fullName: '',
  product: '',
  quantity: 1,
  deliveryDate: '',
  address: '',
  paymentMethod: '',
  agreeTerms: false,
});

const { errors, validate, validateField, clearError } = useFormValidation();
const isSubmitting = ref(false);

const products = [
  { value: 'laptop', label: 'Laptop' },
  { value: 'phone', label: 'Smartphone' },
  { value: 'tablet', label: 'Tablet' },
  { value: 'monitor', label: 'Monitor' },
  { value: 'keyboard', label: 'Keyboard' },
];

const paymentMethods = [
  { value: 'cash', label: 'Cash' },
  { value: 'card', label: 'Credit Card' },
  { value: 'online', label: 'Online Payment' },
];

const handleBlur = (field) => {
  const error = validateField(field, formData.value[field]);
  if (error) {
    errors.value[field] = error;
  } else {
    clearError(field);
  }
};

const resetForm = () => {
  formData.value = {
    fullName: '',
    product: '',
    quantity: 1,
    deliveryDate: '',
    address: '',
    paymentMethod: '',
    agreeTerms: false,
  };
  errors.value = {};
};

const handleSubmit = async () => {
  if (!validate(formData.value)) {
    showError('Please fix errors in the form');
    return;
  }

  isSubmitting.value = true;

  try {
    await api.submitForm('order', formData.value);
    success('Order submitted successfully!');
    resetForm();
    emit('submitted');
  } catch (err) {
    if (err instanceof ApiError) {
      showError(err.message);
    } else {
      showError('An error occurred while submitting the order');
    }
    console.error('Failed to submit form:', err);
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<template>
  <div class="order-form">
    <h2>Order Form</h2>

    <form @submit.prevent="handleSubmit">
      <div class="form-group">
        <label for="fullName">Full Name *</label>
        <input
          id="fullName"
          v-model="formData.fullName"
          type="text"
          @blur="handleBlur('fullName')"
          :class="{ error: errors.fullName }"
        />
        <span v-if="errors.fullName" class="error-message">{{ errors.fullName }}</span>
      </div>

      <div class="form-group">
        <label for="product">Product *</label>
        <select
          id="product"
          v-model="formData.product"
          @blur="handleBlur('product')"
          :class="{ error: errors.product }"
        >
          <option value="">Select a product</option>
          <option v-for="product in products" :key="product.value" :value="product.value">
            {{ product.label }}
          </option>
        </select>
        <span v-if="errors.product" class="error-message">{{ errors.product }}</span>
      </div>

      <div class="form-group">
        <label for="quantity">Quantity *</label>
        <input
          id="quantity"
          v-model.number="formData.quantity"
          type="number"
          min="1"
          max="100"
          @blur="handleBlur('quantity')"
          :class="{ error: errors.quantity }"
        />
        <span v-if="errors.quantity" class="error-message">{{ errors.quantity }}</span>
      </div>

      <div class="form-group">
        <label for="deliveryDate">Delivery Date *</label>
        <input
          id="deliveryDate"
          v-model="formData.deliveryDate"
          type="date"
          @blur="handleBlur('deliveryDate')"
          :class="{ error: errors.deliveryDate }"
        />
        <span v-if="errors.deliveryDate" class="error-message">{{ errors.deliveryDate }}</span>
      </div>

      <div class="form-group">
        <label for="address">Delivery Address *</label>
        <textarea
          id="address"
          v-model="formData.address"
          rows="3"
          @blur="handleBlur('address')"
          :class="{ error: errors.address }"
        ></textarea>
        <span v-if="errors.address" class="error-message">{{ errors.address }}</span>
      </div>

      <div class="form-group">
        <label>Payment Method *</label>
        <div class="radio-group">
          <label v-for="method in paymentMethods" :key="method.value" class="radio-label">
            <input
              type="radio"
              v-model="formData.paymentMethod"
              :value="method.value"
              @change="handleBlur('paymentMethod')"
            />
            {{ method.label }}
          </label>
        </div>
        <span v-if="errors.paymentMethod" class="error-message">{{ errors.paymentMethod }}</span>
      </div>

      <div class="form-group">
        <label class="checkbox-label">
          <input
            type="checkbox"
            v-model="formData.agreeTerms"
            @change="handleBlur('agreeTerms')"
          />
          I agree to the terms *
        </label>
        <span v-if="errors.agreeTerms" class="error-message">{{ errors.agreeTerms }}</span>
      </div>

      <button type="submit" :disabled="isSubmitting" class="submit-btn">
        {{ isSubmitting ? 'Submitting...' : 'Submit Order' }}
      </button>
    </form>
  </div>
</template>

<style scoped>
.order-form {
  max-width: 600px;
  margin: 0 auto;
  padding: 2rem;
  background: #fff;
  border-radius: 12px;
  border: 1px solid #f0f0f0;
}

h2 {
  margin: 0 0 2rem 0;
  color: #1a1a1a;
  font-size: 1.5rem;
  font-weight: 600;
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
  font-size: 0.9rem;
}

input[type="text"],
input[type="number"],
input[type="date"],
select,
textarea {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  font-size: 0.95rem;
  transition: all 0.2s;
  box-sizing: border-box;
  font-family: inherit;
}

input[type="text"]:focus,
input[type="number"]:focus,
input[type="date"]:focus,
select:focus,
textarea:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 3px rgba(76, 175, 80, 0.1);
}

input.error,
select.error,
textarea.error {
  border-color: #f44336;
}

.error-message {
  display: block;
  margin-top: 0.5rem;
  color: #f44336;
  font-size: 0.85rem;
}

.radio-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.radio-label {
  display: flex;
  align-items: center;
  cursor: pointer;
  font-weight: normal;
  padding: 0.75rem 1rem;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  transition: all 0.2s;
  position: relative;
}

.radio-label:hover {
  border-color: #4CAF50;
  background: rgba(76, 175, 80, 0.03);
}

.radio-label:has(input[type="radio"]:checked) {
  border-color: #4CAF50;
  background: rgba(76, 175, 80, 0.08);
}

.radio-label input[type="radio"] {
  appearance: none;
  width: 20px;
  height: 20px;
  border: 2px solid #ccc;
  border-radius: 50%;
  margin-right: 0.75rem;
  cursor: pointer;
  position: relative;
  transition: all 0.2s;
  flex-shrink: 0;
}

.radio-label input[type="radio"]:checked {
  border-color: #4CAF50;
  border-width: 6px;
}

.radio-label input[type="radio"]:hover {
  border-color: #4CAF50;
}

.checkbox-label {
  display: flex;
  align-items: center;
  cursor: pointer;
  font-weight: normal;
  padding: 0.75rem 1rem;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  transition: all 0.2s;
}

.checkbox-label:hover {
  border-color: #4CAF50;
  background: rgba(76, 175, 80, 0.03);
}

.checkbox-label:has(input[type="checkbox"]:checked) {
  border-color: #4CAF50;
  background: rgba(76, 175, 80, 0.08);
}

.checkbox-label input[type="checkbox"] {
  appearance: none;
  width: 20px;
  height: 20px;
  border: 2px solid #ccc;
  border-radius: 4px;
  margin-right: 0.75rem;
  cursor: pointer;
  position: relative;
  transition: all 0.2s;
  flex-shrink: 0;
}

.checkbox-label input[type="checkbox"]:checked {
  border-color: #4CAF50;
  background: #4CAF50;
}

.checkbox-label input[type="checkbox"]:checked::after {
  content: '';
  position: absolute;
  left: 6px;
  top: 2px;
  width: 4px;
  height: 9px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.checkbox-label input[type="checkbox"]:hover {
  border-color: #4CAF50;
}

.submit-btn {
  width: 100%;
  padding: 1rem;
  background: #4CAF50;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  margin-top: 0.5rem;
}

.submit-btn:hover:not(:disabled) {
  background: #45a049;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(76, 175, 80, 0.3);
}

.submit-btn:active:not(:disabled) {
  transform: translateY(0);
}

.submit-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
}

@media (max-width: 768px) {
  .order-form {
    padding: 1.5rem;
    border-radius: 8px;
  }
}
</style>

