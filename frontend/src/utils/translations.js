export const fieldTranslations = {
  fullName: 'Full Name',
  product: 'Product',
  quantity: 'Quantity',
  deliveryDate: 'Delivery Date',
  address: 'Delivery Address',
  paymentMethod: 'Payment Method',
  agreeTerms: 'Terms Agreement',
};

export const productTranslations = {
  laptop: 'Laptop',
  phone: 'Smartphone',
  tablet: 'Tablet',
  monitor: 'Monitor',
  keyboard: 'Keyboard',
};

export const paymentMethodTranslations = {
  cash: 'Cash',
  card: 'Credit Card',
  online: 'Online Payment',
};

export function translateFieldName(field) {
  return fieldTranslations[field] || field;
}

export function translateValue(field, value) {
  if (typeof value === 'boolean') {
    return value ? 'Yes' : 'No';
  }
  
  if (field === 'product' && productTranslations[value]) {
    return productTranslations[value];
  }
  
  if (field === 'paymentMethod' && paymentMethodTranslations[value]) {
    return paymentMethodTranslations[value];
  }
  
  return String(value);
}

