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
namespace org.camunda.bpm.engine.rest.dto.task
{
	using FormData = org.camunda.bpm.engine.form.FormData;

	/// 
	/// <summary>
	/// @author nico.rehwaldt
	/// </summary>
	public class FormDto
	{

	  private string key;
	  private string contextPath;

	  public virtual string Key
	  {
		  set
		  {
			this.key = value;
		  }
		  get
		  {
			return key;
		  }
	  }


	  public virtual string ContextPath
	  {
		  set
		  {
			this.contextPath = value;
		  }
		  get
		  {
			return contextPath;
		  }
	  }


	  public static FormDto fromFormData(FormData formData)
	  {
		FormDto dto = new FormDto();

		if (formData != null)
		{
		  dto.key = formData.FormKey;
		}

		return dto;
	  }
	}

}