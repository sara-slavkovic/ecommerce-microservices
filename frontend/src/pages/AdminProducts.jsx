import { useEffect, useState } from "react";
import ProductForm from "../components/ProductForm";
import AdminProductRow from "../components/AdminProductRow";
import Spinner from "../components/Spinner";
import { getAllProducts, getAllCategories } from "../api/catalogService";
import { getErrorMessage } from "../api/errorHandling";

function AdminProducts() {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const loadProducts = () => {
    getAllProducts()
      .then(setProducts)
      .catch(() => {});
  };

  useEffect(() => {
    Promise.all([getAllProducts(), getAllCategories()])
      .then(([productsData, categoriesData]) => {
        setProducts(productsData);
        setCategories(categoriesData);
      })
      .catch((err) =>
        setError(getErrorMessage(err, "Admin panel is currently unavailable.")),
      )
      .finally(() => setLoading(false));
  }, []);

  const handleUpdated = (updatedProduct) => {
    setProducts((prev) =>
      prev.map((p) => (p.id === updatedProduct.id ? updatedProduct : p)),
    );
  };

  const handleDeleted = (id) => {
    setProducts((prev) => prev.filter((p) => p.id !== id));
  };

  if (loading) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Spinner text="Loading admin panel..." />
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <div
          style={{
            maxWidth: "500px",
            margin: "80px auto",
            padding: "30px",
            backgroundColor: "var(--card-bg)",
            borderRadius: "8px",
            textAlign: "center",
          }}
        >
          <h2 style={{ marginTop: 0 }}>Oops!</h2>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{ minHeight: "100vh" }}>
      <div style={{ padding: "2rem", maxWidth: "800px", margin: "0 auto" }}>
        <h1
          style={{
            borderBottom: "1px solid var(--accent)",
            paddingBottom: "10px",
          }}
        >
          Manage Products
        </h1>

        <ProductForm categories={categories} onCreated={loadProducts} />

        <h3>Existing Products</h3>
        {products.map((p) => (
          <AdminProductRow
            key={p.id}
            product={p}
            categories={categories}
            onDeleted={handleDeleted}
            onUpdated={handleUpdated}
          />
        ))}
      </div>
    </div>
  );
}

export default AdminProducts;
