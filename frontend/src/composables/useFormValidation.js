import { ref } from 'vue';

const validationRules = {
  fullName: [
    { 
      test: (v) => v && v.trim().length >= 2,
      message: 'Full name must contain at least 2 characters'
    },
  ],
  product: [
    { 
      test: (v) => !!v,
      message: 'Please select a product'
    },
  ],
  quantity: [
    { test: (v) => v >= 1, message: 'Quantity must be greater than 0' },
    { test: (v) => v <= 100, message: 'Quantity cannot exceed 100' },
  ],
  deliveryDate: [
    { test: (v) => !!v, message: 'Please select a delivery date' },
    { 
      test: (v) => {
        const date = new Date(v);
        const today = new Date(); today.setHours(0,0,0,0);
        return date >= today;
      },
      message: 'Delivery date cannot be in the past'
    },
  ],
  address: [
    { 
      test: (v) => v && v.trim().length >= 10,
      message: 'Address must contain at least 10 characters'
    },
  ],
  paymentMethod: [
    { 
      test: (v) => !!v,
      message: 'Please select a payment method'
    },
  ],
  agreeTerms: [
    { test: (v) => v === true, message: 'You must agree to the terms' },
  ],
};

export function useFormValidation() {
  const errors = ref({});

  const validateField = (field, value) => {
    const rules = validationRules[field];
    if (!rules) return null;

    for (const rule of rules) {
      if (!rule.test(value)) {
        return rule.message;
      }
    }
    return null;
  };

  const validate = (formData) => {
    errors.value = {};
    let isValid = true;

    for (const field in formData) {
      const error = validateField(field, formData[field]);
      if (error) {
        errors.value[field] = error;
        isValid = false;
      }
    }

    return isValid;
  };

  const clearError = (field) => {
    delete errors.value[field];
  };

  return { errors, validate, validateField, clearError };
}

