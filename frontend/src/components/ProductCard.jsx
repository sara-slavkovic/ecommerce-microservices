function ProductCard({ product }) {
  const imageUrl = product.imageUrl && product.imageUrl.startsWith('images')
    ? `https://localhost:7038/${product.imageUrl}`
    : 'https://via.placeholder.com/230?text=No+Image';

  return (
    <div style={{
      backgroundColor: 'var(--card-bg)',
      borderRadius: '8px',
      padding: '15px',
      textAlign: 'center',
      boxShadow: '0 4px 6px rgba(0,0,0,0.05)'
    }}>
      <img src={imageUrl} alt={product.name} style={{ width: '100%', aspectRatio: '1 / 1', objectFit: 'cover', borderRadius: '5px' }} />
      <h3 style={{ margin: '15px 0 5px 0' }}>{product.name}</h3>
      <p style={{ margin: '0 0 10px 0', fontStyle: 'italic' }}>{product.brand}</p>
      <h3 style={{ margin: '0 0 15px 0' }}>${product.price.toFixed(2)}</h3>
      <button style={{ width: '100%' }}>Add to Cart</button>
    </div>
  );
}

export default ProductCard;