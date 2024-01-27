using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPoolableProp {
    public GameObject gameObject { get; }

    // isAvailable should be set internally, but interfaces don't allow for private setters
    public bool isAvailable { get; set; }
    public void activate(Dictionary<string, object> args);
    public void deactivate();
}

public class PropObjectPool : MonoBehaviour {

    static PropObjectPool store;
    public static PropObjectPool instance {
        get {
            if (store == null) {
                store = FindObjectOfType(typeof(PropObjectPool)) as PropObjectPool;
                if (store == null) {
                    store = new GameObject(typeof(PropObjectPool).ToString()).AddComponent<PropObjectPool>();
                }
                store.initialize();
            }
            return store;
        }
    }

    Dictionary<string, List<IPoolableProp>> pools;
    Dictionary<string, GameObject> propTypes;

    void initialize() {
        pools = new Dictionary<string, List<IPoolableProp>>();
        propTypes = new Dictionary<string, GameObject>();
    }

    public static void addPropPrefabToStore(string propName, GameObject prop) {
        if (!instance.propTypes.ContainsKey(propName)) {
            instance.propTypes.Add(propName, prop);
            if (!instance.pools.ContainsKey(propName)) {
                instance.pools.Add(propName, new List<IPoolableProp>());
            }
        }
    }

    public static IPoolableProp getFirstAvailableProp(string propName) {
        if (instance.pools.TryGetValue(propName, out List<IPoolableProp> props)) {
            IPoolableProp foundProp = props.FirstOrDefault(prop => prop.isAvailable);
            if (foundProp != null) {
                return foundProp;
            }
            IPoolableProp createdProp = Instantiate(instance.propTypes[propName], null).GetComponent<IPoolableProp>();
            props.Add(createdProp);
            return createdProp;
        } else {
            Debug.Log("prop used before it was added to the store");
            return null;
        }
    }
}
