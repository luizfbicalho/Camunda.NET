using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.core.model
{

	/// <summary>
	/// Properties that maps property keys to values. The properties cannot contain
	/// duplicate property names; each property name can map to at most one value.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
	public class Properties
	{

	  protected internal readonly IDictionary<string, object> properties;

	  public Properties() : this(new Dictionary<string, object>())
	  {
	  }

	  public Properties(IDictionary<string, object> properties)
	  {
		this.properties = properties;
	  }

	  /// <summary>
	  /// Returns the value to which the specified property key is mapped, or
	  /// <code>null</code> if this properties contains no mapping for the property key.
	  /// </summary>
	  /// <param name="property">
	  ///          the property key whose associated value is to be returned </param>
	  /// <returns> the value to which the specified property key is mapped, or
	  ///         <code>null</code> if this properties contains no mapping for the property key </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T get(PropertyKey<T> property)
	  public virtual T get<T>(PropertyKey<T> property)
	  {
		return (T) properties[property.Name];
	  }

	  /// <summary>
	  /// Returns the list to which the specified property key is mapped, or
	  /// an empty list if this properties contains no mapping for the property key.
	  /// Note that the empty list is not mapped to the property key.
	  /// </summary>
	  /// <param name="property">
	  ///          the property key whose associated list is to be returned </param>
	  /// <returns> the list to which the specified property key is mapped, or
	  ///         an empty list if this properties contains no mapping for the property key
	  /// </returns>
	  /// <seealso cref= #addListItem(PropertyListKey, Object) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> java.util.List<T> get(PropertyListKey<T> property)
	  public virtual IList<T> get<T>(PropertyListKey<T> property)
	  {
		if (contains(property))
		{
		  return (IList<T>) properties[property.Name];
		}
		else
		{
		  return new List<T>();
		}
	  }

	  /// <summary>
	  /// Returns the map to which the specified property key is mapped, or
	  /// an empty map if this properties contains no mapping for the property key.
	  /// Note that the empty map is not mapped to the property key.
	  /// </summary>
	  /// <param name="property">
	  ///          the property key whose associated map is to be returned </param>
	  /// <returns> the map to which the specified property key is mapped, or
	  ///         an empty map if this properties contains no mapping for the property key
	  /// </returns>
	  /// <seealso cref= #putMapEntry(PropertyMapKey, Object, Object) </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <K, V> java.util.Map<K, V> get(PropertyMapKey<K, V> property)
	  public virtual IDictionary<K, V> get<K, V>(PropertyMapKey<K, V> property)
	  {
		if (contains(property))
		{
		  return (IDictionary<K, V>) properties[property.Name];
		}
		else
		{
		  return new Dictionary<K, V>();
		}
	  }

	  /// <summary>
	  /// Associates the specified value with the specified property key. If the properties previously contained a mapping for the property key, the old
	  /// value is replaced by the specified value.
	  /// </summary>
	  /// @param <T>
	  ///          the type of the value </param>
	  /// <param name="property">
	  ///          the property key with which the specified value is to be associated </param>
	  /// <param name="value">
	  ///          the value to be associated with the specified property key </param>
	  public virtual void set<T>(PropertyKey<T> property, T value)
	  {
		properties[property.Name] = value;
	  }

	  /// <summary>
	  /// Associates the specified list with the specified property key. If the properties previously contained a mapping for the property key, the old
	  /// value is replaced by the specified list.
	  /// </summary>
	  /// @param <T>
	  ///          the type of elements in the list </param>
	  /// <param name="property">
	  ///          the property key with which the specified list is to be associated </param>
	  /// <param name="value">
	  ///          the list to be associated with the specified property key </param>
	  public virtual void set<T>(PropertyListKey<T> property, IList<T> value)
	  {
		properties[property.Name] = value;
	  }

	  /// <summary>
	  /// Associates the specified map with the specified property key. If the properties previously contained a mapping for the property key, the old
	  /// value is replaced by the specified map.
	  /// </summary>
	  /// @param <K>
	  ///          the type of keys maintained by the map </param>
	  /// @param <V>
	  ///          the type of mapped values </param>
	  /// <param name="property">
	  ///          the property key with which the specified map is to be associated </param>
	  /// <param name="value">
	  ///          the map to be associated with the specified property key </param>
	  public virtual void set<K, V>(PropertyMapKey<K, V> property, IDictionary<K, V> value)
	  {
		properties[property.Name] = value;
	  }

	  /// <summary>
	  /// Append the value to the list to which the specified property key is mapped. If
	  /// this properties contains no mapping for the property key, the value append to
	  /// a new list witch is associate the the specified property key.
	  /// </summary>
	  /// @param <T>
	  ///          the type of elements in the list </param>
	  /// <param name="property">
	  ///          the property key whose associated list is to be added </param>
	  /// <param name="value">
	  ///          the value to be appended to list </param>
	  public virtual void addListItem<T>(PropertyListKey<T> property, T value)
	  {
		IList<T> list = get(property);
		list.Add(value);

		if (!contains(property))
		{
		  set(property, list);
		}
	  }

	  /// <summary>
	  /// Insert the value to the map to which the specified property key is mapped. If
	  /// this properties contains no mapping for the property key, the value insert to
	  /// a new map witch is associate the the specified property key.
	  /// </summary>
	  /// @param <K>
	  ///          the type of keys maintained by the map </param>
	  /// @param <V>
	  ///          the type of mapped values </param>
	  /// <param name="property">
	  ///          the property key whose associated list is to be added </param>
	  /// <param name="value">
	  ///          the value to be appended to list </param>
	  public virtual void putMapEntry<K, V>(PropertyMapKey<K, V> property, K key, V value)
	  {
		IDictionary<K, V> map = get(property);

		if (!property.allowsOverwrite() && map.ContainsKey(key))
		{
		  throw new ProcessEngineException("Cannot overwrite property key " + key + ". Key already exists");
		}

		map[key] = value;

		if (!contains(property))
		{
		  set(property, map);
		}
	  }

	  /// <summary>
	  /// Returns <code>true</code> if this properties contains a mapping for the specified property key.
	  /// </summary>
	  /// <param name="property">
	  ///            the property key whose presence is to be tested </param>
	  /// <returns> <code>true</code> if this properties contains a mapping for the specified property key </returns>
	  public virtual bool contains<T1>(PropertyKey<T1> property)
	  {
		return properties.ContainsKey(property.Name);
	  }

	  /// <summary>
	  /// Returns <code>true</code> if this properties contains a mapping for the specified property key.
	  /// </summary>
	  /// <param name="property">
	  ///            the property key whose presence is to be tested </param>
	  /// <returns> <code>true</code> if this properties contains a mapping for the specified property key </returns>
	  public virtual bool contains<T1>(PropertyListKey<T1> property)
	  {
		return properties.ContainsKey(property.Name);
	  }

	  /// <summary>
	  /// Returns <code>true</code> if this properties contains a mapping for the specified property key.
	  /// </summary>
	  /// <param name="property">
	  ///            the property key whose presence is to be tested </param>
	  /// <returns> <code>true</code> if this properties contains a mapping for the specified property key </returns>
	  public virtual bool contains<T1>(PropertyMapKey<T1> property)
	  {
		return properties.ContainsKey(property.Name);
	  }

	  /// <summary>
	  /// Returns a map view of this properties. Changes to the map are not reflected
	  /// to the properties.
	  /// </summary>
	  /// <returns> a map view of this properties </returns>
	  public virtual IDictionary<string, object> toMap()
	  {
		return new Dictionary<string, object>(properties);
	  }

	  public override string ToString()
	  {
		return "Properties [properties=" + properties + "]";
	  }

	}

}