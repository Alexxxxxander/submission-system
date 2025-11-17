import { describe, it, expect, test } from 'vitest';
import { useFormValidation } from '../composables/useFormValidation';

describe('useFormValidation', () => {
  const tomorrow = new Date(Date.now() + 86400000).toISOString().split('T')[0];
  const yesterday = new Date(Date.now() - 86400000).toISOString().split('T')[0];

  describe('validateField', () => {
    test.each([
      ['fullName', '', 'Full name must contain at least 2 characters'],
      ['fullName', 'A', 'Full name must contain at least 2 characters'],
      ['fullName', 'John Doe', null],
      ['product', '', 'Please select a product'],
      ['product', 'laptop', null],
      ['quantity', 0, 'Quantity must be greater than 0'],
      ['quantity', 101, 'Quantity cannot exceed 100'],
      ['quantity', 50, null],
      ['deliveryDate', '', 'Please select a delivery date'],
      ['deliveryDate', yesterday, 'Delivery date cannot be in the past'],
      ['deliveryDate', tomorrow, null],
      ['address', '', 'Address must contain at least 10 characters'],
      ['address', 'Short', 'Address must contain at least 10 characters'],
      ['address', '123 Main St, New York, NY 10001', null],
      ['paymentMethod', '', 'Please select a payment method'],
      ['paymentMethod', 'card', null],
      ['agreeTerms', false, 'You must agree to the terms'],
      ['agreeTerms', true, null],
    ])('field %s with value %s', (field, value, expected) => {
      const { validateField } = useFormValidation();
      expect(validateField(field, value)).toBe(expected);
    });

    it('returns null for unknown field', () => {
      const { validateField } = useFormValidation();
      expect(validateField('unknownField', 'any')).toBe(null);
    });
  });

  describe('validate', () => {
    it('returns true and no errors for valid form', () => {
      const { validate, errors } = useFormValidation();

      const validForm = {
        fullName: 'John Doe',
        product: 'laptop',
        quantity: 1,
        deliveryDate: tomorrow,
        address: '123 Main St, New York, NY 10001',
        paymentMethod: 'card',
        agreeTerms: true,
      };

      const result = validate(validForm);
      expect(result).toBe(true);
      expect(errors.value).toEqual({});
    });

    it('returns false and all errors for invalid form', () => {
      const { validate, errors } = useFormValidation();

      const invalidForm = {
        fullName: '',
        product: '',
        quantity: 0,
        deliveryDate: '',
        address: '',
        paymentMethod: '',
        agreeTerms: false,
      };

      const result = validate(invalidForm);
      expect(result).toBe(false);
      expect(Object.keys(errors.value)).toEqual([
        'fullName',
        'product',
        'quantity',
        'deliveryDate',
        'address',
        'paymentMethod',
        'agreeTerms',
      ]);
    });

    it('handles partial invalid form', () => {
      const { validate, errors } = useFormValidation();

      const partialForm = {
        fullName: 'John Doe',
        product: '',
        quantity: 2,
        deliveryDate: tomorrow,
        address: '123 Main St, New York, NY 10001',
        paymentMethod: '',
        agreeTerms: true,
      };

      const result = validate(partialForm);
      expect(result).toBe(false);
      expect(errors.value).toHaveProperty('product');
      expect(errors.value).toHaveProperty('paymentMethod');
      expect(errors.value).not.toHaveProperty('fullName');
    });
  });

  describe('clearError', () => {
    it('clears individual errors correctly', () => {
      const { errors, clearError } = useFormValidation();

      errors.value = {
        fullName: 'Error',
        product: 'Error',
      };

      clearError('fullName');

      expect(errors.value.fullName).toBeUndefined();
      expect(errors.value.product).toBe('Error');
    });
  });
});
