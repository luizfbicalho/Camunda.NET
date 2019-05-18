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
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using ModelInstanceValidator = org.camunda.bpm.model.xml.impl.validation.ModelInstanceValidator;
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using ModelElementValidator = org.camunda.bpm.model.xml.validation.ModelElementValidator;
	using ValidationResults = org.camunda.bpm.model.xml.validation.ValidationResults;


	/// <summary>
	/// An instance of a model
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public class ModelInstanceImpl : ModelInstance
	{

	  protected internal readonly DomDocument document;
	  protected internal ModelImpl model;
	  protected internal readonly ModelBuilder modelBuilder;

	  public ModelInstanceImpl(ModelImpl model, ModelBuilder modelBuilder, DomDocument document)
	  {
		this.model = model;
		this.modelBuilder = modelBuilder;
		this.document = document;
	  }

	  public virtual DomDocument Document
	  {
		  get
		  {
			return document;
		  }
	  }

	  public virtual ModelElementInstance DocumentElement
	  {
		  get
		  {
			DomElement rootElement = document.RootElement;
			if (rootElement != null)
			{
			  return ModelUtil.getModelElement(rootElement, this);
			}
			else
			{
			  return null;
			}
		  }
		  set
		  {
			ModelUtil.ensureInstanceOf(value, typeof(ModelElementInstanceImpl));
			DomElement domElement = value.DomElement;
			document.RootElement = domElement;
		  }
	  }


	  public virtual T newInstance<T>(Type<T> type) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		return newInstance(type, null);
	  }

	  public virtual T newInstance<T>(Type<T> type, string id) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ModelElementType modelElementType = model.getType(type);
		if (modelElementType != null)
		{
		  return newInstance(modelElementType, id);
		}
		else
		{
		  throw new ModelException("Cannot create instance of ModelType " + type + ": no such type registered.");
		}
	  }

	  public virtual T newInstance<T>(ModelElementType type) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		return newInstance(type, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.model.xml.instance.ModelElementInstance> T newInstance(org.camunda.bpm.model.xml.type.ModelElementType type, String id)
	  public virtual T newInstance<T>(ModelElementType type, string id) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		ModelElementInstance modelElementInstance = type.newInstance(this);
		if (!string.ReferenceEquals(id, null) && id.Length > 0)
		{
		  ModelUtil.setNewIdentifier(type, modelElementInstance, id, false);
		}
		else
		{
		  ModelUtil.setGeneratedUniqueIdentifier(type, modelElementInstance, false);
		}
		return (T) modelElementInstance;
	  }

	  public virtual Model Model
	  {
		  get
		  {
			return model;
		  }
	  }

	  public virtual ModelElementType registerGenericType(string namespaceUri, string localName)
	  {
		ModelElementType elementType = model.getTypeForName(namespaceUri, localName);
		if (elementType == null)
		{
		  elementType = modelBuilder.defineGenericType(localName, namespaceUri);
		  model = (ModelImpl) modelBuilder.build();
		}
		return elementType;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.model.xml.instance.ModelElementInstance> T getModelElementById(String id)
	  public virtual T getModelElementById<T>(string id) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		if (string.ReferenceEquals(id, null))
		{
		  return null;
		}

		DomElement element = document.getElementById(id);
		if (element != null)
		{
		  return (T) ModelUtil.getModelElement(element, this);
		}
		else
		{
		  return null;
		}
	  }

	  public virtual ICollection<ModelElementInstance> getModelElementsByType(ModelElementType type)
	  {
		ICollection<ModelElementType> extendingTypes = type.AllExtendingTypes;

		IList<ModelElementInstance> instances = new List<ModelElementInstance>();
		foreach (ModelElementType modelElementType in extendingTypes)
		{
		  if (!modelElementType.Abstract)
		  {
			((IList<ModelElementInstance>)instances).AddRange(modelElementType.getInstances(this));
		  }
		}
		return instances;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.model.xml.instance.ModelElementInstance> java.util.Collection<T> getModelElementsByType(Class<T> referencingClass)
	  public virtual ICollection<T> getModelElementsByType<T>(Type<T> referencingClass) where T : org.camunda.bpm.model.xml.instance.ModelElementInstance
	  {
		return (ICollection<T>) getModelElementsByType(Model.getType(referencingClass));
	  }

	  public virtual ModelInstance clone()
	  {
		  return new ModelInstanceImpl(model, modelBuilder, document.clone());
	  }

	  public virtual ValidationResults validate<T1>(ICollection<T1> validators)
	  {
		return (new ModelInstanceValidator(this, validators)).validate();
	  }

	}

}