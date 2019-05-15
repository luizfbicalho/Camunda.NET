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
namespace org.camunda.bpm.engine.rest.cache
{
	public interface Cache
	{

	  /// <summary>
	  /// Put a resource into the cache.
	  /// </summary>
	  /// <param name="id"> the id of the resource </param>
	  /// <param name="resource"> the resource to cache </param>
	  void put(string id, object resource);

	  /// <summary>
	  /// Get a resource by id.
	  /// </summary>
	  /// <param name="id"> the id of the resource </param>
	  /// <returns> the resource or null if non is found or the resource time to live expired </returns>
	  object get(string id);

	  /// <summary>
	  /// Destroy cache.
	  /// </summary>
	  void destroy();

	}

}