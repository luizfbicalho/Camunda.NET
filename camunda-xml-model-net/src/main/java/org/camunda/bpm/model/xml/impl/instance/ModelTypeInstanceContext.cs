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
namespace org.camunda.bpm.model.xml.impl.instance
{
	using ModelElementTypeImpl = org.camunda.bpm.model.xml.impl.type.ModelElementTypeImpl;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public sealed class ModelTypeInstanceContext
	{

	  private readonly ModelInstanceImpl model;
	  private readonly DomElement domElement;
	  private readonly ModelElementTypeImpl modelType;

	  public ModelTypeInstanceContext(DomElement domElement, ModelInstanceImpl model, ModelElementTypeImpl modelType)
	  {
		this.domElement = domElement;
		this.model = model;
		this.modelType = modelType;
	  }

	  /// <returns> the dom element </returns>
	  public DomElement DomElement
	  {
		  get
		  {
			return domElement;
		  }
	  }

	  /// <returns> the model </returns>
	  public ModelInstanceImpl Model
	  {
		  get
		  {
			return model;
		  }
	  }

	  /// <returns> the modelType </returns>
	  public ModelElementTypeImpl ModelType
	  {
		  get
		  {
			return modelType;
		  }
	  }

	}

}