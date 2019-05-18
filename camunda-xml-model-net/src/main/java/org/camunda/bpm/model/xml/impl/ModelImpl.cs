using System;
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
namespace org.camunda.bpm.model.xml.impl
{

	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using QName = org.camunda.bpm.model.xml.impl.util.QName;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;

	/// <summary>
	/// A model contains all defined types and the relationship between them.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModelImpl : Model
	{

	  private readonly IDictionary<QName, ModelElementType> typesByName = new Dictionary<QName, ModelElementType>();
	  private readonly IDictionary<Type, ModelElementType> typesByClass = new Dictionary<Type, ModelElementType>();
	  private readonly string modelName;

	  protected internal readonly IDictionary<string, string> actualNsToAlternative = new Dictionary<string, string>();
	  protected internal readonly IDictionary<string, string> alternativeNsToActual = new Dictionary<string, string>();

	  /// <summary>
	  /// Create a new <seealso cref="Model"/> with a model name. </summary>
	  /// <param name="modelName">  the model name to identify the model </param>
	  public ModelImpl(string modelName)
	  {
		this.modelName = modelName;
	  }

	  /// <summary>
	  /// Declares an alternative namespace for an actual so that during lookup of elements/attributes both will be considered.
	  /// This can be used if a newer namespaces replaces an older one but XML files with the old one should still be parseable. </summary>
	  /// <param name="alternativeNs"> </param>
	  /// <param name="actualNs"> </param>
	  /// <exception cref="IllegalArgumentException"> if the alternative is already used or if the actual namespace has an alternative </exception>
	  public virtual void declareAlternativeNamespace(string alternativeNs, string actualNs)
	  {
		if (actualNsToAlternative.ContainsKey(actualNs) || alternativeNsToActual.ContainsKey(alternativeNs))
		{
		  throw new System.ArgumentException("Cannot register two alternatives for one namespace! Actual Ns: " + actualNs + " second alternative: " + alternativeNs);
		}
		actualNsToAlternative[actualNs] = alternativeNs;
		alternativeNsToActual[alternativeNs] = actualNs;
	  }

	  public virtual void undeclareAlternativeNamespace(string alternativeNs)
	  {
		if (!alternativeNsToActual.ContainsKey(alternativeNs))
		{
		  return;
		}
		string actual = alternativeNsToActual.Remove(alternativeNs);
		actualNsToAlternative.Remove(actual);
	  }

	  public virtual string getAlternativeNamespace(string actualNs)
	  {
		return actualNsToAlternative[actualNs];
	  }

	  public virtual string getActualNamespace(string alternativeNs)
	  {
		return alternativeNsToActual[alternativeNs];
	  }

	  public virtual ICollection<ModelElementType> Types
	  {
		  get
		  {
			return new List<ModelElementType>(typesByName.Values);
		  }
	  }

	  public virtual ModelElementType getType(Type instanceClass)
	  {
		return typesByClass[instanceClass];
	  }

	  public virtual ModelElementType getTypeForName(string typeName)
	  {
		return getTypeForName(null, typeName);
	  }

	  public virtual ModelElementType getTypeForName(string namespaceUri, string typeName)
	  {
		return typesByName[ModelUtil.getQName(namespaceUri, typeName)];
	  }

	  /// <summary>
	  /// Registers a <seealso cref="ModelElementType"/> in this <seealso cref="Model"/>.
	  /// </summary>
	  /// <param name="modelElementType">  the element type to register </param>
	  /// <param name="instanceType">  the instance class of the type to register </param>
	  public virtual void registerType(ModelElementType modelElementType, Type instanceType)
	  {
		QName qName = ModelUtil.getQName(modelElementType.TypeNamespace, modelElementType.TypeName);
		typesByName[qName] = modelElementType;
		typesByClass[instanceType] = modelElementType;
	  }

	  public virtual string ModelName
	  {
		  get
		  {
			return modelName;
		  }
	  }

	  public override int GetHashCode()
	  {
		int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(modelName, null)) ? 0 : modelName.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		ModelImpl other = (ModelImpl) obj;
		if (string.ReferenceEquals(modelName, null))
		{
		  if (!string.ReferenceEquals(other.modelName, null))
		  {
			return false;
		  }
		}
		else if (!modelName.Equals(other.modelName))
		{
		  return false;
		}
		return true;
	  }

	}

}