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
namespace org.camunda.bpm.model.xml.impl.type.reference
{
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;

	public class UriElementReferenceImpl<Target, Source> : ElementReferenceImpl<Target, Source> where Target : org.camunda.bpm.model.xml.instance.ModelElementInstance where Source : org.camunda.bpm.model.xml.instance.ModelElementInstance
	{

	  public UriElementReferenceImpl(ChildElement<Source> referenceSourceCollection) : base(referenceSourceCollection)
	  {
	  }

	  public override string getReferenceIdentifier(ModelElementInstance referenceSourceElement)
	  {
		// TODO: implement something more robust (CAM-4028)
		string identifier = referenceSourceElement.getAttributeValue("href");
		if (!string.ReferenceEquals(identifier, null))
		{
		  string[] parts = identifier.Split("#", true);
		  if (parts.Length > 1)
		  {
			return parts[parts.Length - 1];
		  }
		  else
		  {
			return parts[0];
		  }
		}
		else
		{
		  return null;
		}
	  }

	  protected internal override void setReferenceIdentifier(ModelElementInstance referenceSourceElement, string referenceIdentifier)
	  {
		// TODO: implement something more robust (CAM-4028)
		referenceSourceElement.setAttributeValue("href", "#" + referenceIdentifier);
	  }

	}

}