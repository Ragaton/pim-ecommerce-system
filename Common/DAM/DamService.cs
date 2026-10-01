/*using DAM_Server.Service;

namespace DAM.DamService;
public class DamService : IDamService {

    public struct AssetResult {
        public string ProductId;
        public string Url;
    }

    public DamService() {
        Services.Initialize();
    }
    
    public List<string> GetAllAssets() {
        return new List<string>();
    }
    
    /// <summary>
    /// Featch a single product asset using the product ID
    /// </summary>
    /// <param name="productId">Id of the product</param>
    /// <returns>A struct with the ProductId and the url</returns>
    public AssetResult GetProductAsset(string productId) {
        Services.Database.GetProductAsset(productId);
        
        return new AssetResult() {
            ProductId = productId,
            Url = "https://example.com/asset.jpg"
        };
    }

    
}
*/