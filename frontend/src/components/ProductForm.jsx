import { useState, useRef } from "react";
import {
  uploadProductImage,
  createProduct,
  getImageUrl,
} from "../api/catalogService";
import { getErrorMessage } from "../api/errorHandling";
import { useToast } from "../hooks/useToast";

function ProductForm({ categories, onCreated }) {
  const showToast = useToast();
  const [step, setStep] = useState("name"); // 'name' | 'details'
  const [uploading, setUploading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const fileInputRef = useRef(null);

  const [formData, setFormData] = useState({
    sku: "",
    name: "",
    brand: "",
    description: "",
    price: "",
    imageUrl: "",
    isActive: true,
    categoryId: "",
    initialStockQuantity: "",
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleImageSelect = async (e) => {
    const file = e.target.files[0];
    if (!file || !formData.name.trim()) return;

    setUploading(true);
    setError("");
    try {
      const imageUrl = await uploadProductImage(file, formData.name);
      setFormData((prev) => ({ ...prev, imageUrl }));
      setStep("details");
    } catch (err) {
      setError(getErrorMessage(err, "Image upload failed. Please try again."));
    } finally {
      setUploading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSaving(true);
    try {
      const dto = {
        ...formData,
        price: parseFloat(formData.price),
        initialStockQuantity: parseInt(formData.initialStockQuantity, 10),
      };
      await createProduct(dto);
      showToast(`${formData.name} created successfully!`, "success");
      setFormData({
        sku: "",
        name: "",
        brand: "",
        description: "",
        price: "",
        imageUrl: "",
        isActive: true,
        categoryId: "",
        initialStockQuantity: "",
      });
      setStep("name");
      onCreated();
    } catch (err) {
      setError(
        getErrorMessage(err, "Failed to create product. Please try again."),
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <div
      style={{
        backgroundColor: "var(--card-bg)",
        borderRadius: "8px",
        padding: "25px",
        marginBottom: "2rem",
      }}
    >
      <h3 style={{ marginTop: 0 }}>Add New Product</h3>
      {error && <p style={{ color: "#b33" }}>{error}</p>}

      {step === "name" && (
        <div>
          <input
            type="text"
            name="name"
            placeholder="Product Name"
            value={formData.name}
            onChange={handleChange}
            required
          />
          <label
            style={{ display: "block", marginTop: "10px", fontSize: "0.9rem" }}
          >
            {formData.name.trim()
              ? "Choose an image to continue:"
              : "Enter a name first, then choose an image"}
          </label>
          <input
            type="file"
            accept="image/*"
            ref={fileInputRef}
            onChange={handleImageSelect}
            disabled={!formData.name.trim() || uploading}
            style={{ display: "none" }}
          />
          <button
            type="button"
            onClick={() => fileInputRef.current.click()}
            disabled={!formData.name.trim() || uploading}
            style={{ marginTop: "5px" }}
          >
            Choose Image
          </button>
          {uploading && (
            <p style={{ fontSize: "0.9rem" }}>Uploading image...</p>
          )}
        </div>
      )}

      {step === "details" && (
        <form onSubmit={handleSubmit}>
          <div
            style={{
              display: "flex",
              gap: "15px",
              alignItems: "center",
              marginBottom: "10px",
            }}
          >
            <img
              src={getImageUrl(formData.imageUrl)}
              alt="preview"
              style={{
                width: "60px",
                height: "60px",
                objectFit: "cover",
                borderRadius: "5px",
              }}
            />
            <strong>{formData.name}</strong>
          </div>

          <input
            type="text"
            name="sku"
            placeholder="SKU"
            value={formData.sku}
            onChange={handleChange}
            required
          />
          <input
            type="text"
            name="brand"
            placeholder="Brand"
            value={formData.brand}
            onChange={handleChange}
            required
          />
          <textarea
            name="description"
            placeholder="Description"
            value={formData.description}
            onChange={handleChange}
            rows={3}
            required
          />
          <input
            type="number"
            step="0.01"
            name="price"
            placeholder="Price"
            value={formData.price}
            onChange={handleChange}
            required
          />
          <select
            name="categoryId"
            value={formData.categoryId}
            onChange={handleChange}
            required
            style={{
              width: "100%",
              padding: "10px",
              border: "1px solid var(--accent)",
              borderRadius: "5px",
              marginBottom: "15px",
            }}
          >
            <option value="">Select Category</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
          <input
            type="number"
            name="initialStockQuantity"
            placeholder="Initial Stock Quantity"
            value={formData.initialStockQuantity}
            onChange={handleChange}
            required
          />

          <button type="submit" disabled={saving} style={{ width: "100%" }}>
            {saving ? "Creating..." : "Create Product"}
          </button>
        </form>
      )}
    </div>
  );
}

export default ProductForm;
